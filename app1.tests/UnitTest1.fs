module app1.tests

open NUnit.Framework

open Extractor

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Test1 () =
    Assert.Pass()

[<TestFixture>]
type TestClass () =
    [<Test>]
    member __.passingMethod () =
        Assert.True(true)

    [<Test>]
    member __.failingMethod () =
        Assert.True(false)


[<Test>]
let extract_time_test () =
    let expected = "11:03"
    let actual = extract_time "at 11:03"
    Assert.That(actual, Is.EqualTo(expected))
