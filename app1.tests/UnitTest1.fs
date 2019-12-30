module app1.tests

open NUnit.Framework

open Extractor

[<SetUp>]
let Setup () =
    ()

[<Test>]
let ``always pass test`` () =
    Assert.Pass()

[<TestFixture>]
type TestClass () =
    [<Test>]
    member __.passingMethod () =
        Assert.True(true)

    [<Test>]
    member __.``always failing test`` () =
        Assert.True(false)


[<TestFixture>]
type ExtractTimeTest () =
    [<Test>]
    member __.``extract correct time`` () =
        let expected = "11:03"
        let actual = extract_time "at 11:03"
        Assert.That(actual, Is.EqualTo(expected))

    [<Test>]
    member __.``return empty string instead of time on rubbish text`` () =
        let expected = ""
        let actual = extract_time "11:03"
        Assert.That(actual, Is.EqualTo(expected))
