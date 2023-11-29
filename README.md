# MSTestSmells

This is a set of Visual Studio analyzers and codefixes for detecting test smells in projects made with the testing framework MSTest.

## Detected Smells:
| Test smell      | Description                                                                               |
|------------------------|------------------------------------------------------------------------------------|
| Assertion Roulette     | The test has multiple asserts without descriptions.                                |
| Conditional Test Logic | The test has control statements, or it's result depends on a control statement.    |
| Duplicate Assert       | The test checks for the same condition multiple times within the same test method. |
| Eager Test             | The has multiple assertions, and they check the results of more than one method.   |
| Empty Test             | The test contains no executable statements.                                        |
| Exception Handling     | The test throws or catches an exception.                                           |
| General Fixture        | The setup fixture sets up variables used in only some tests.                       |
| Ignored Test           | The test has the \[ignore\] attribute.                                               |
| Magic Number Test      | The test has assertions with literal values as arguments.                          |
| Mystery Guest          | The test uses external resources, such as a file or database.                      |
| Redundant Assertion    | The test has an assertion which checks the equality of an object with itself.      |
| Sleepy Test            | The test uses the sleep() function.                                                |
| Unknown Test           | The test does not have any assertion.                                              |
| Obvious Fail           | The test has IsTrue(false) or IsFalse(true) assertions.          |

## Fixed Smells
| Test smell      | Description                                                                               |
|------------------------|------------------------------------------------------------------------------------|
| Assertion Roulette     | Add message argument.                                |
| Eager Test             | Extract the test into multiple tests.   |
| Empty Test             | Add NotImplementedException.                                        |
| Exception Handling     | The test throws or catches an exception.                                           |
| Ignored Test           | Remove \[ignore\] attribute.                                               |
| Magic Number Test      | Extract loval constant.                          |
| Redundant Assertion    | Remove redundant assertion.      |
| Obvious Fail           | Replace assertion with Assert.Fail().          |

## .editorconfig Settings
Apart from the default warning level of each analyzer, there are 2 additional configurations that can be set:
### dotnet_diagnostic.MysteryGuest.IgnoredFiles
`dotnet_diagnostic.MysteryGuest.IgnoredFiles = file1, file2, etc`

This setting allows for a comma-separated list of filenames to ignore when checking for external files. Specifically, the detector checks if any string literal in the method contains any of the defined ignored files.

### dotnet_diagnostic.UnknownTest.CustomAssertions
`dotnet_diagnostic.UnknownTest.CustomAssertions = methodname1, methodname2, etc`

This setting allows for a comma-separated list of helper assertion methods to take into account when checking for tests without assertions. Only the name must be provided. For example, `TestUtils.ListChecker(IEnumerable<int> intlist)` should be passed as simply `ListChecker`.

## Command line tool
Apart from the in-editor analyzers, a command line analysis tool is included.

The tool is run as follows: `Testsmells.Console.exe -s <path to solution>`
There are multiple parameters that can be given to the tool in order to further customize the results:
  - **--solution, -s:** Absolute path to the \texttt{.sln} file of the solution to be analyzed.
  - **--output, -o:** Path for CSV output. The tool will create a CSV file with the given filename, and save a list of each encountered smell diagnostic (that was not hidden).  If a path is not given, the information will be displayed in the command console.
  - **--method_output, -m:** Path for method summary. The tool will create a CSV file with the given filename, and save a list of each method affected by at least one smell, detailing the amount of each smell encountered in it.
  - **--method_list_output, -l:** Path for method list. The tool will create a CSV file with the given filename, and save a list of each test method in then solution.
  - **--config, -c:** Path for a JSON configuration file. This configuration lets a developer define the smells' severities and define global configuration values.
  - **--ignore_default_config, -i:** Flag for ignoring the configuration values defined in the project in favour of the ones set in the JSON configuration file. 


The JSON configuration file expected syntax is as follows (all fields are optional):
```json
{
    "dotnet_diagnostic.MysteryGuest.IgnoredFiles": "file1, file2, etc",
    "dotnet_diagnostic.UnknownTest.CustomAssertions": "methodname1, methodname2, etc",
    "severity":
    {
        "UnknownTest": "hidden",
        "MysteryGuest": "error",
        ...
    }
}
```
