# MI-GEN

**Goal:** Create optimization eliminating unnecesary null checks from C# Source.



Project uses both published (Syntax API) and prototype API (CFG API), because CFG APIs are not yet published, this projects depends on dev (nightly) or custom (local) builds of roslyn.

[Syntax API](Roslyn_syntax_API.md) contains simple description of the public API

[Control Flow Graph API](Roslyn_CFG_API.md)  has my notes written when exploring the current state of CFG APIs in Roslyn.

[Samples](Samples.md) - samples of CFG/IOperations exposed by Roslyn APIs

#### Other docs (maybe blogsposts in the future)

[Getting nightly builds of roslyn](Getting_latest_roslyn.md) 

[Using localy build of (some) Roslyn libraries](Local_builds.md)



## TODO

- [ ] Conditional build with Roslyn locally linked vs myget linking
- [ ] Handle sub-regions
- [ ] Init implementation of abstract interpretation

### Otázky

- [ ] V rámci abstraktní interpretace se volá Merge vždy, když dojde ke změně stavu, nebo jen na začátku BB? Má tedy svaz mít dvě funkce - merge a update?

  ```c#
  var a = default(string); //UNKNOWN
  a = "a";				//NEVER_NULL
  a = null;				//Merge (NEVER_NULL, ALWAYS_NULL) -> SOMETIMES_NULl ???
  a = "b";				//SOMETIMES_NULL	
  if (a != null) 			 //a není null, ale pokud používáme merge -> SOMETIMES_NULL
  ```

- [ ] Je svaz navrhnutý správně?

- [ ] Blok zařadím do fronty jen pokud by jeho vstupní stav byl jiný, než posledně (viz cache stavů)? 

- [ ] Když začnu interpretovat blok, je počáteční stav při jeho interpretaci ten z uložení do fronty, nebo ten z cache (nebo budou vždy stejné a do fronty stačí ukládat BB bez počátečního stavu)?

## Resources

https://github.com/Cybermaxs/awesome-analyzers

https://johnkoerner.com/archive/ - good blog about analyzers

https://renniestechblog.com/information/45-modifying-roslyn-step-12-anonymizing-the-property-names - modifying roslyn

Learn Roslyn Now!: https://www.youtube.com/watch?v=wXXHd8gYqVg&index=1&list=PLxk7xaZWBdUT23QfaQTCJDG6Q1xx6uHdG

