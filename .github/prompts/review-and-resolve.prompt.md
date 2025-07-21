---
mode: agent
---

# Code Review and Resolution Prompt

## Task Definition
Review the codebase for compliance with established coding standards and resolve any violations found. Focus on high-impact improvements that enhance code quality, maintainability, and adherence to project conventions.

## Core Review Principles

**Every change must provide genuine business value.** Before making any modification, ask:

1. **Does this solve a real problem?**
2. **Does this improve security, performance, or maintainability?**
3. **Is this change necessary for compliance with established standards?**

## What NOT to Add

### ❌ Artificial Complexity
- **Fake async patterns** - Don't convert synchronous operations to async without real async work
- **Unnecessary abstractions** - Don't add interfaces or patterns unless there's a clear need
- **Ceremonial code** - Don't add boilerplate that doesn't serve a purpose

### ❌ Obvious Documentation
- **Self-explanatory logging** - Don't log "Home page accessed" or similar obvious operations
- **Trivial comments** - Don't document what the code obviously does
- **Over-documentation** - Don't add XML docs to simple, self-explanatory methods

### ❌ Cosmetic Changes
- **Style-only modifications** - Let .editorconfig handle formatting
- **Renaming without purpose** - Don't change names unless clarity is significantly improved
- **Structural changes for preference** - Don't reorganize code without clear benefit

## What TO Focus On

### ✅ High-Impact Improvements
- **Security vulnerabilities** - Input validation, authentication, authorization
- **Performance bottlenecks** - Inefficient queries, unnecessary allocations
- **Error handling** - Proper exception handling and meaningful error messages
- **Maintainability issues** - Complex methods, tight coupling, unclear logic

### ✅ Compliance Requirements
- **Instruction file violations** - Code that doesn't follow established patterns
- **Framework best practices** - Proper use of ASP.NET Core, xUnit, etc.
- **Code quality standards** - Nullable reference types, proper async patterns

## Requirements

### 1. **Review Against Instruction Files**
Analyze code compliance with all instruction files in `.github/instructions/`:

- `aspnet-core.instructions.md` - ASP.NET Core patterns and best practices
- `csharp-coding.instructions.md` - C# coding standards and conventions
- `testing-strategy.instructions.md` - Testing framework usage and patterns
- `razor-views.instructions.md` - View structure and accessibility guidelines
- `documentation.instructions.md` - Documentation standards and requirements
- `frameworks-and-libraries.instructions.md` - Approved technology stack compliance

### 2. **Code Quality Focus Areas**

**High Priority Violations:**
- Security and performance issues
- Critical violations of instruction file guidelines
- Breaking changes to established patterns

**Medium Priority Improvements:**
- Missing documentation or tests
- Inconsistencies with project conventions
- Code quality improvements

**Low Priority (Skip for this review):**
- Minor formatting issues handled by .editorconfig
- Cosmetic improvements without functional impact

### 3. **Resolution Strategy**

**For Each Violation Found:**
1. **Categorize Impact** - High/Medium/Low based on security, performance, and maintainability
2. **Verify Business Value** - Does this change solve a real problem or improve the codebase meaningfully?
3. **Provide Specific Fix** - Include exact code changes with before/after examples
4. **Reference Guidelines** - Cite specific instruction file and section
5. **Validate Changes** - Ensure fixes align with project patterns and don't introduce regressions

**Implementation Approach:**
- Make focused, atomic changes - one logical improvement per file edit
- Follow established patterns from existing codebase
- Maintain backward compatibility unless explicitly breaking changes are needed
- Add appropriate tests for any new functionality introduced
- **Avoid unnecessary additions** - don't add logging, async patterns, or documentation without clear value

## Success Criteria

### ✅ **Compliance Achieved**
- All high-priority violations from instruction files resolved
- Code adheres to established project patterns and conventions
- No breaking changes to existing functionality

### ✅ **Validation Passed**
Execute validation sequence from `dotnet-commands.instructions.md`:

**Expected Results:**
- Clean build with zero warnings
- All tests passing
- Improved code maintainability

## Constraints

### Core Review Principles

**Every change must provide genuine business value.** Before making any modification, ask:

1. **Does this solve a real problem?**
2. **Does this improve security, performance, or maintainability?**
3. **Is this change necessary for compliance with established standards?**

### **Scope Limitations**
- **Focus on high-confidence changes only** - avoid modifications where requirements are ambiguous
- **Preserve existing functionality** - ensure no behavioral changes unless explicitly required
- **Maintain API compatibility** - don't break existing public interfaces

### **Change Guidelines**
- **One logical change per file edit** - keep modifications focused and reviewable
- **Include context in changes** - provide 3-5 lines of surrounding code for clarity
- **Test after each change** - validate that modifications don't introduce regressions
- **Reference instruction files** - cite specific guidelines when making changes
