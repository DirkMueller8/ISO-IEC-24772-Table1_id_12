## Good Coding Principles acc. to ISO/IEC 24772-1:2024

### Standard ### 
**ISO/IEC 24772-1**  
Programming languages — Avoiding vulnerabilities in programming languages —
Part 1: Language-independent catalogue of vulnerabilities   

### Good Coding Recommendation  
"*Prohibit the modification of loop control variables inside the loop  body.*", in Table 1, Number 12, on p. 28   

### Theory around the Example  
#### Why the rule matters ####  
- **Correctness** — modifying the loop control variable breaks the loop invariant. That easily produces off-by-one logic, skipped or duplicated iterations, and index-out-of-range errors.  
- **Termination safety** — changes to the control variable can make the loop fail to terminate (infinite loop) or terminate prematurely in ways that are hard to reason about.  
- **Readability & maintainability** — loop behavior becomes non-local: callers and future maintainers must mentally track extra mutations. This increases bugs during refactoring.  
- **Debugging & testing cost** — tests and debuggers assume simple iteration progression. Mutating the control variable hides the real control flow and makes reproducing failures harder.  
- **Static analysis & verification** — analyzers and formal proofs assume loop counters behave predictably. Mutations defeat many automated checks.  
- **Concurrency & reentrancy** — if the loop control state is shared or captured (lambda/closure), modifying it increases the chance of race conditions and subtle bugs.  
- **Security surface** — unexpected iteration can bypass validation or repeat sensitive operations.  

#### Concrete failure modes (examples) ####  
- **Off-by-one**: decrementing the index on error can cause the next iteration to write the same slot twice or to write before the first element.  
- **Index-out-of-range**: accidental extra increment/decrement combinations can push the index past bounds.  
- **Infinite loop**: a catch/retry that decrements and an unconditional increment can interact to never advance the effective progress.  
- **Hidden side effects**: mixing break/continue with manual index adjustments is fragile.  

#### Problems in my code ####  
- `ProblematicLoop()` mutates i inside the loop (brittle and error-prone).  
- `SafeLoop()` uses a dedicated inner validation loop and `int.TryParse` — it preserves the outer loop’s control variable and is far easier to reason about.  

#### Best practices (apply these instead) ####  
- Never modify the loop control variable inside the loop body.  
- If you must retry input or validation for the same logical iteration, use an inner loop (validation loop) or a helper method that returns a result.  
- Prefer `int.TryParse` and returning error codes/optional results rather than manipulating indices.  
- Use for when you need an index and foreach when you only need elements (and never modify the underlying collection during enumeration).  
- Keep loop invariants obvious and documented; limit side effects inside the loop body.  
- When cancellation or retry is needed, surface it with exceptions, return values, or explicit state variables — not by changing the loop index.  