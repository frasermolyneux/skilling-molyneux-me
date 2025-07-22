# GitHub Copilot Instructions

## General Guidelines

- At any point if there is something that is unclear or you have a low confidence in; I want you to ask clarifying questions back to me.
- DO *NOT* create summary markdown files of a change set unless there is a specific request for it.

## Instructions Directory

**Always consult and adhere to applicable instruction files** in the `.github/instructions/` directory:

- **Review relevant instruction files** before starting any task
- **Follow documented procedures** and commands from instruction files
- **Use specified tools and workflows** as outlined in the instructions
- **Maintain consistency** with established patterns and standards
- **Reference instruction files** when explaining processes or providing guidance

## Development Flow

When working on this project, follow this standardized development flow:

### 1. **Understand & Clarify**
- **Read the request carefully** and identify what needs to be implemented or changed
- **Ask clarifying questions** if any requirements are unclear or ambiguous
- **Confirm scope** - what files/components will be affected
- **Verify understanding** - summarize what you plan to do before starting
- **Question every change** - Does this provide genuine business value?

### 2. **Plan & Design**
- **Analyze existing code** structure and patterns
- **Follow established conventions** from the codebase
- **Consider dependencies** and impacts on other components
- **Plan testing approach** for the changes
- **Avoid artificial complexity** - don't add async patterns without real async work
- **Focus on high-impact improvements** - security, performance, maintainability

### 3. **Implement Changes**
- **Make focused changes** - one logical change per file edit
- **Follow coding standards** defined in .editorconfig
- **Add appropriate documentation** and comments only when they provide genuine value
- **Ensure code quality** - proper error handling, validation, etc.
- **Avoid unnecessary logging** - don't log obvious operations like "page accessed"
- **Keep methods simple** - use async only when there's genuine async work

### 4. **Validate Changes**
After making any code changes, **always validate** using the commands from `.github/instructions/dotnet-commands.instructions.md`:

```bash
# Standard validation sequence
dotnet clean src/MX.Skilling.sln && dotnet restore src/MX.Skilling.sln && dotnet build src/MX.Skilling.sln && dotnet test src/MX.Skilling.sln && dotnet format src/MX.Skilling.sln --verify-no-changes
```

### 5. **Verify & Confirm**
- **Check build output** for any warnings or errors
- **Run tests** to ensure nothing is broken
- **Verify functionality** meets the original requirements
- **Report results** - summarize what was changed and validation status

### 6. **Documentation**
- **Update relevant documentation** if API or behavior changes
- **Add inline comments** for complex logic
- **Update README** or instruction files if needed

## Quality Gates

Before considering any work complete:

✅ **Code Quality**
- All code follows .editorconfig standards
- No build warnings or errors
- Proper error handling and validation

✅ **Testing**
- All existing tests pass
- New functionality has appropriate tests
- Test coverage is maintained or improved

✅ **Documentation**
- Code is self-documenting with clear naming
- Complex logic has explanatory comments
- Public APIs have XML documentation

✅ **Validation**
- Clean build with no warnings
- All tests passing
- Application runs successfully

## Error Handling

If validation fails:
1. **Stop and analyze** the error messages
2. **Fix issues** before proceeding
3. **Re-run validation** to confirm fixes
4. **Report** what was fixed and why it failed initially
