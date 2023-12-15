Reduce entropy of C# code.

Assumes e.g. `clang-format` run before and after, so it doesn't need to worry about exact spaces.

Applies the following transformations:

- Capitalize the first word of the first line comment in a block thereof
- Sort case labels in a section
- Sort case sections in a switch statement
