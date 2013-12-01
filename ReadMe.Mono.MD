What is Jurassic?
=================
>>Jurassic is an implementation of the ECMAScript language and runtime. It aims to provide the best performing and most standards-compliant implementation of JavaScript for .NET. Jurassic is not intended for end-users; instead it is intended to be integrated into .NET programs. If you are the author of a .NET program, you can use Jurassic to compile and execute JavaScript code.

> This is a fork and cloned with original commits from the project:
> https://jurassic.codeplex.com

Issues:
>Report any issues on the original project site, 
>this is only a fork for Mono patching : 
>https://jurassic.codeplex.com/workitem/list/basic

* XBuild Debug:
```sh
xbuild /p:Configuration=Debug Jurassic.Mono.sln 
```
* XBuild Clean Debug:
```sh
xbuild /p:Configuration=Debug /t:Clean Jurassic.Mono.sln 
```
* Xbuild Release:
```sh
xbuild /p:Configuration=Release Jurassic.Mono.sln 
```
* XBuild Clean Release
```sh
xbuild /p:Configuration=Release /t:Clean Jurassic.Mono.sln 
```

Version
----
* Based on 2.1 commit 976fc0aa3c15, Sep 30, 2013
https://jurassic.codeplex.com/SourceControl/changeset/976fc0aa3c15

Tech
-----------
* Mono 3.2.5 .Net 4.5 / C#5 

OS-X/*nix Setup & Test
--------------
```sh
git clone http://github.com:sushihangover/Jurassic.git
cd Jurassic
xbuild /p:Configuration=Debug Jurassic.Mono.sln
xbuild /p:Configuration=Release Jurassic.Mono.sln
pushd NUnitTests/bin/Debug/
nunit-console.exe "Unit Tests.dll"
popd
pushp Benchmarker/bin/Release/
mono Benchmarker.exe
```

License
----

Microsoft Public License (Ms-PL) https://jurassic.codeplex.com/license

*Free Software, Hell Yeah!*

