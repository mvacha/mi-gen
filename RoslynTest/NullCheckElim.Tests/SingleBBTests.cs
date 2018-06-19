using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssert;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System.Linq;
using static NullCheckElim.Tests.RoslynHelpers;

namespace NullCheckElim.Tests
{
    [TestClass]
    public class SingleBBTests
    {
        private EvaluatedRegion<NullLattice, NullLatticeValue> InterpretSingleBB(ControlFlowGraph cfg)
        {
            var mainBB = cfg.Blocks.Single(bb => bb.Kind == BasicBlockKind.Block);
            var mainRegion = cfg.Root.NestedRegions[0];
            var mainEvalRegion = EvaluatedRegion<NullLattice, NullLatticeValue>.CreateFromRegion(cfg.Root.NestedRegions[0]);

            //Nested regions not yet supported
            mainRegion.NestedRegions.ShouldBeEmpty();

            var analysis = new NullCheckElimination();
            return analysis.InterpretBB(mainBB, mainEvalRegion);
        }

        private void TestSingleVar(string statements, NullLatticeValue resultValue)
        {
            var source = WrapInMain(statements);
            var cfg = CompileGetCFG(source);
            var result = InterpretSingleBB(cfg);

            result.Symbols.Count.ShouldBeEqualTo(1);
            result.Symbols.First().Value.Value.ShouldBeEqualTo(resultValue);
        }

        [TestMethod]
        public void SingleVarLiteralAssignmentsNull()
        {
            var code = "string str = null;";
            TestSingleVar(code, NullLatticeValue.Null);

            code = "string str = default(string);";
            TestSingleVar(code, NullLatticeValue.Null);

            code = "string str = default;";  //YAY: C# 7 feature
            TestSingleVar(code, NullLatticeValue.Null);

            code = @"
                string str;
                str = ""ahoj"";
                str = default(string);
            ";

            TestSingleVar(code, NullLatticeValue.Null);

        }

        [TestMethod]
        public void SingleVarLiteralAssignmentsNotNull()
        {
            var code = "string str = \"ahoj\";";
            TestSingleVar(code, NullLatticeValue.NotNull);

            code = "string str = new string('a', 10);";
            TestSingleVar(code, NullLatticeValue.NotNull);

            code = @"
                string str;
                str = null;
                str = default; //YAY: C# 7 feature
                str = default(string);
                str = ""ahoj"";
            ";

            TestSingleVar(code, NullLatticeValue.NotNull);

        }

        [TestMethod]
        public void ShouldIgnoreNonNullableTypes()
        {
            var code = "int str = 10;";
            TestSingleVar(code, NullLatticeValue.Unknown);
        }

        [TestMethod]
        public void ShouldHandleReferenceAssignments()
        {
            var code = @"
                string str = null;
                string abc = ""abc"";
                abc = str;
            ";

            var source = WrapInMain(code);
            var cfg = CompileGetCFG(source);
            var result = InterpretSingleBB(cfg);
            var symbols = result.Symbols.ToSearchableDictionary();

            symbols.Count.ShouldBeEqualTo(2);
            symbols["str"].ShouldBeEqualTo(NullLatticeValue.Null);
            symbols["abc"].ShouldBeEqualTo(NullLatticeValue.Null);
        }

        [TestMethod]
        public void ShouldHandleStringEmpty()
        {
            var code = @"
                string str = null;
                string abc = string.Empty;
                str = abc;
            ";

            var source = WrapInMain(code);
            var cfg = CompileGetCFG(source);
            var result = InterpretSingleBB(cfg);
            var symbols = result.Symbols.ToSearchableDictionary();

            symbols.Count.ShouldBeEqualTo(2);
            symbols["str"].ShouldBeEqualTo(NullLatticeValue.NotNull);
            symbols["abc"].ShouldBeEqualTo(NullLatticeValue.NotNull);
        }
    }
}
