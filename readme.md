# IC10Transpiler

This is a tool for generating IC10 code for the [Stationeers](https://rocketwerkz.itch.io/stationeers) game from C-like language.

## Program structure

Programs are written in procedural style. There's no formal Main method or any other entry point, program is transpiled statement-by-statement from the source. Functions might be defined in and called from the program using C-like syntax:

```c++
var a = 10;
a = factorial(a);

function factorial(a){
    var result = 1;
    while(a > 0){
        result *= a;
        a--;
    }
    return result;
}
```

## Statements
Statements form the program. Each statement must be terminated with `;` (except for block-like statements, like loops or conditionals).

### Definitions
```c++
define a = 123;
```
Definitions are translated directly into IC10 definitions. However, they still share namespace with other symbols, like variables or functions. In the resulting IC10 code definitions are always bubble to the program start.
### Declarations
```c++
var a = 123, b;
```
These constructs declare variables. Variable scope is limited either to the main program body (outside function definition) or to the function. Variables could be used before declaration, however their initialization is not completed by that time. Uninitialized variables' values should not be read as doing so produces unpredictable results.

All variables have type `float`, nothing else is supported by IC10.

Variables are mapped to IC10 registers, starting from `r15` to `r2`. Thus, you cannot have more than 14 variables in scope.

### Function declarations and return
```c++
function name(arg1, arg2){
    return 0;
}
```
All functions should have return statement in all execution branches. Internally, return values are passed to the caller via `r0`, and arguments are passed to the function via stack. Arguments share namespace with variables inside functions and use IC10 registers too, so no more than 14 arguments+variables for any function.

### Assignments and compound assignments
```c++
a = a + b;
a += b;
a++;
```
Please note, that `++` and `--` operators are actually statements, they cannot be used on right-hand side of an assignment.

### Loops
```c++
while(a>0){ break; }
do{}while(a>0);
```
Loops exist only in `while` form. There's no `for` loop. Loop body is executed while condition is nonzero. Additionally `break` statement might be used to exit the innermost loop early.
### Conditionals
```c++
if(a>0)b=1;
if(a>0)b=1;else {c=1;}
```
Just usual conditional blocks without any caveats.

## Expressions
Expressions are used as right-hand side of assignments, function call arguments or conditions for loops and conditional statements.

### Literals
```
123
"abcd"
'abcd'
```
Numeric literals are used as-is. Double-quoted literals are used for designating enum-like stuff in IC10, i.e. LogicType. Single-quoted literals designating string hashes, so they are used for DeviceHash, ItemHash etc.

### Basic maths
```c++
a+b
a-b
a*b
a/b
a%b
(a+b)*c
```
These are supported math operators.

### Logical expressions
```
true
false
a>b
a<b
a>=b
a<=b
a==b
a!=b
a>b && b>c
a>b || b>c
```
These expressions must only be used as conditions in loops and conditional statements. No other expressions are accepted there. Both left and right sides of comparison may contain other expression of any complexity.

### Function calls
```c++
name(1, variable, "Literal", 'HashLiteral')
```
Before calling function all register variables are saved on stack and restored after returning.
Upon function entry arguments are popped from stack, and `ra` is saved there.
Before return `ra` is restored, and return value is ensured to be stored in `r0`.

## Builtin functions

Some IC10 functionality is implemented in form of builtin functions. Most notably these are maths and device I/O.
Math functions can use any argument types and work as you should expect from them:
```
rand()
abs(a)
ceil(a)
exp(a)
floor(a)
log(a)
round(a)
sqrt(a)
trunc(a)
sin(a)
cos(a)
asin(a)
acos(a)
atan(a)
tan(a)
min(a,b)
max(a,b)
```

Device I/O functions have several limitations on their argument types, e.g. only literals or definitions could be used as `Hash` arguments and `LogicType` arguments. `device` arguments should use double quote syntax. `mode` argument in `get*Batch*` functions accept either literal number 0-3 or double-quoted mnemonic `"Maximum"`.
```
get(device, logicType)
getByReference(ref, logicType)
getBatch(typeHash, logicType, mode)
getBatchNamed(typeHash, nameHash, logicType, mode)
getSlotBatch(typeHash, slotId, logicType, mode)
getSlotBatchNamed(typeHash, nameHash, slotId, logicType, mode)
set(device, logicType, value)
setBatch(typeHash, logicType, value)
setBatchNamed(typeHash, nameHash, logicType, value)
setSlotBatch(typeHash, slotId, logicType, value)
```

## Extending
To add new language constructs, like comments or `for` loops, you should start with editing [grammar file](Parser/grammar.cs.atg). After that, grammar should be compiled with [Coco/R](https://ssw.jku.at/Research/Projects/Coco/). Semantic actions inside grammar should map to AST nodes in the [AST folder](AST/).

If you need to change how code is generated you should look into `Emit` methods of the [`Program`](AST/Program.cs) class and `Statement` inheritors.

If you want to add more builtin functions, add a new class into [AST/Builtin](AST/Builtin/) folder, inherited from the [`BuiltinFunction`](AST/BuiltinFunction.cs) and implement `Emit` method.

