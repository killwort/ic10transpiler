using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using Ic10Transpiler.AST;


using System;
using System.CodeDom.Compiler;

namespace Ic10Transpiler.Parser {



#region COCO/R Generated Code

[GeneratedCode("Coco/R","1.0.0")]
internal class Parser {
	public const int _EOF = 0;
	public const int _ident = 1;
	public const int _number = 2;
	public const int _string = 3;
	public const int _stringBq = 4;
	public const int _internalMultiplicativeOp = 5;
	public const int _internalAdditiveOp = 6;
	public const int _compoundAssignmentOp = 7;
	public const int _logicalOp = 8;
	public const int _logicalBinaryOp = 9;
	public const int maxT = 30;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public IErrors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

private readonly List<Statement> result = new List<Statement>();
private List<ISymbol> symtable = new List<ISymbol>();
private readonly List<Define> defines = new List<Define>();
internal static Program Parse(string src){
	var errorCollector=new ErrorCollector();
	var p=new Parser(new Scanner(new StringBuffer(src)),errorCollector);
	p.Parse();
	errorCollector.ThrowErrors();
	return new Program(p.defines, p.symtable, p.result);
}

private static string Unquote(string val){
return UnescapeRegex.Replace(val.Substring(1, val.Length - 2), ev => ev.Value.Substring(1));
}
private static readonly Regex UnescapeRegex = new Regex(@"\\.", RegexOptions.Compiled);



	public Parser(Scanner scanner, IErrors errors) {
		this.scanner = scanner;
		this.errors = errors??new ErrorCollector();
	}

	void SynErr (int n) {
		if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public void SemErr (string msg) {
		if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
		errDist = 0;
	}
	
	void Get () {
		for (;;) {
			t = la;
			la = scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = t;
		}
	}
	
	void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}
	
	bool StartOf (int s) {
		return set[s, la.kind];
	}
	
	void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}


	bool WeakSeparator(int n, int syFol, int repFol) {
		int kind = la.kind;
		if (kind == n) {Get(); return true;}
		else if (StartOf(repFol)) {return false;}
		else {
			SynErr(n);
			while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind])) {
				Get();
				kind = la.kind;
			}
			return StartOf(syFol);
		}
	}

	
	void VariableDeclaration(out ArrayList declarations, out ArrayList initializations) {
		declarations = new ArrayList(); initializations = new ArrayList(); 
		Expect(10);
		OneVariableDeclaration(initializations, out var d);
		declarations.Add(d); 
		while (la.kind == 11) {
			Get();
			OneVariableDeclaration(initializations, out d);
			declarations.Add(d); 
		}
		Expect(12);
	}

	void OneVariableDeclaration(ArrayList initializations, out VarDeclaration vd) {
		Expect(1);
		vd = new VarDeclaration(t, t.val); 
		if (la.kind == 13) {
			Get();
			Expression(out var initVal);
			initializations.Add(new Assignment(t, vd.Name, initVal)); 
		}
	}

	void Expression(out Expr expr) {
		AdditiveExpression(out expr);
	}

	void Const(out string literal) {
		literal = null; 
		if (la.kind == 2) {
			Get();
			literal = t.val; 
		} else if (la.kind == 4) {
			Get();
			literal = "HASH(\"" + Unquote(t.val) + "\")"; 
		} else if (la.kind == 3) {
			Get();
			literal = Unquote(t.val); 
		} else SynErr(31);
	}

	void Definition(out Define declaration) {
		Expect(14);
		Expect(1);
		declaration = new Define(t, t.val); 
		Expect(13);
		Const(out var c);
		declaration.Value = c; 
	}

	void AdditiveExpression(out Expr expr) {
		MultiplicativeExpression(out var first);
		expr = first; 
		while (la.kind == 6) {
			Get();
			var op = t.val; 
			MultiplicativeExpression(out var next);
			expr = new BinaryExpression(t, expr, op, next); 
		}
	}

	void MultiplicativeExpression(out Expr expr) {
		OperandExpression(out var first);
		expr = first; 
		while (la.kind == 5) {
			Get();
			var op = t.val; 
			OperandExpression(out var next);
			expr = new BinaryExpression(t, expr, op, next); 
		}
	}

	void OperandExpression(out Expr expr) {
		expr = null; 
		if (la.kind == 15) {
			Get();
			AdditiveExpression(out expr);
			Expect(16);
		} else if (la.kind == 2 || la.kind == 3 || la.kind == 4) {
			Const(out var c);
			expr = new ConstValue(t, c); 
		} else if (la.kind == 1) {
			Get();
			var id = t.val; 
			if (la.kind == 15) {
				var args = new List<Expr>(); 
				Get();
				if (StartOf(1)) {
					Expression(out var firstArg);
					args.Add(firstArg); 
					while (la.kind == 11) {
						Get();
						Expression(out var nextArg);
						args.Add(nextArg); 
					}
				}
				Expect(16);
				expr = new FunctionCall(t, id, args); 
			} else if (StartOf(2)) {
				expr = new SymbolRef(t, id); 
			} else SynErr(32);
		} else SynErr(33);
	}

	void FunctionDeclaration(out FunctionDeclaration fn) {
		Expect(17);
		Expect(1);
		fn = new FunctionDeclaration(t, t.val); var oldSt = symtable; symtable = fn.Symtable; 
		Expect(15);
		if (la.kind == 1) {
			Get();
			fn.Symtable.Add(new Argument(t, t.val)); 
			while (la.kind == 11) {
				Get();
				Expect(1);
				fn.Symtable.Add(new Argument(t, t.val)); 
			}
		}
		Expect(16);
		Expect(18);
		StatementList(out var ss);
		fn.Body.AddRange(ss.Cast<Statement>()); symtable = oldSt; 
		Expect(19);
	}

	void StatementList(out ArrayList statements) {
		statements = new ArrayList(); 
		Statement(out var s);
		if(s!=null)statements.Add(s); 
		while (StartOf(3)) {
			Statement(out s);
			if(s!=null)statements.Add(s); 
		}
	}

	void TopLevelStatement(out Statement statement) {
		statement = null; 
		if (la.kind == 17) {
			FunctionDeclaration(out var fn);
			statement = fn; 
		} else if (la.kind == 14) {
			Definition(out var declaration);
			defines.Add(declaration); 
			Expect(12);
		} else if (StartOf(3)) {
			Statement(out statement);
		} else SynErr(34);
	}

	void Statement(out Statement statement) {
		statement = null; 
		switch (la.kind) {
		case 1: {
			Get();
			var id = t.val; 
			if (la.kind == 13) {
				Get();
				Expression(out var rhs);
				statement = new Assignment(t, id, rhs); 
			} else if (la.kind == 7) {
				Get();
				var op = t.val.Substring(0,1); 
				Expression(out var rhs);
				statement = new Assignment(t, id, new BinaryExpression(t, new SymbolRef(t, id), op, rhs)); 
			} else if (la.kind == 22) {
				Get();
				statement = new Assignment(t, id, new BinaryExpression(t, new SymbolRef(t, id), "+", new ConstValue(t,"1"))); 
			} else if (la.kind == 23) {
				Get();
				statement = new Assignment(t, id, new BinaryExpression(t, new SymbolRef(t, id), "-", new ConstValue(t,"1"))); 
			} else if (la.kind == 15) {
				var args = new List<Expr>(); 
				Get();
				if (StartOf(1)) {
					Expression(out var firstArg);
					args.Add(firstArg); 
					while (la.kind == 11) {
						Get();
						Expression(out var nextArg);
						args.Add(nextArg); 
					}
				}
				Expect(16);
				statement = new FunctionCall(t, id, args, true); 
			} else SynErr(35);
			Expect(12);
			break;
		}
		case 24: {
			Get();
			Expect(15);
			LogicalExpression(out var cond);
			Expect(16);
			Statement(out var body);
			statement = new LoopStatement(t, body){ Precondition = cond }; 
			break;
		}
		case 25: {
			Get();
			Statement(out var body);
			Expect(24);
			Expect(15);
			LogicalExpression(out var cond);
			Expect(16);
			statement = new LoopStatement(t, body){ Postcondition = cond }; 
			break;
		}
		case 26: {
			Get();
			statement = new BreakStatement(t); 
			Expect(12);
			break;
		}
		case 27: {
			Get();
			Expect(15);
			LogicalExpression(out var cond);
			Expect(16);
			Statement(out var trueBody);
			statement = new Conditional(t, cond, trueBody); 
			if (la.kind == 28) {
				Get();
				Statement(out var falseBody);
				((Conditional)statement).False = falseBody; 
			}
			break;
		}
		case 18: {
			Get();
			if (StartOf(3)) {
				StatementList(out var ss);
				statement = new Block(t, ss.Cast<Statement>()); 
			}
			Expect(19);
			break;
		}
		case 29: {
			Get();
			Expression(out var rv);
			statement = new Return(t, rv); 
			Expect(12);
			break;
		}
		case 10: {
			VariableDeclaration(out var declarations, out var initializations);
			symtable.AddRange(declarations.Cast<VarDeclaration>()); statement = new Block(t, initializations.Cast<Assignment>()); 
			break;
		}
		case 12: {
			Get();
			break;
		}
		default: SynErr(36); break;
		}
	}

	void LogicalExpression(out LogicalExpression expr) {
		expr = null; 
		if (StartOf(1)) {
			LogicalComparison(out var c);
			expr = c; 
			while (la.kind == 9) {
				Get();
				var op = t.val; 
				LogicalComparison(out c);
				expr = new LogicalJunction(t, expr, op, c); 
			}
		} else if (la.kind == 20) {
			Get();
			expr = new LogicalConstant(t, true); 
		} else if (la.kind == 21) {
			Get();
			expr = new LogicalConstant(t, false); 
		} else SynErr(37);
	}

	void LogicalComparison(out LogicalExpression expr) {
		expr = null; 
		if (StartOf(1)) {
			Expression(out var lhs);
			Expect(8);
			var op = t.val; 
			Expression(out var rhs);
			expr = new LogicalComparison(t, lhs, op, rhs); 
		} else if (la.kind == 15) {
			Get();
			LogicalExpression(out expr);
			Expect(16);
		} else SynErr(38);
	}

	void Ic10cProgram() {
		TopLevelStatement(out var s);
		if(s!=null)result.Add(s); 
		while (StartOf(4)) {
			TopLevelStatement(out s);
			if(s!=null)result.Add(s); 
		}
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		Ic10cProgram();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_T,_T, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_T, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_x,_x,_x, _x,_T,_T,_x, _T,_T,_x,_T, _T,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_T,_x,_x},
		{_x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_T,_x, _T,_x,_T,_x, _x,_T,_T,_x, _x,_x,_x,_x, _T,_T,_T,_T, _x,_T,_x,_x}

	};
} // end Parser

[GeneratedCode("Coco/R","1.0.0")]
internal class ErrorCollector : IErrors, IEnumerable<Error>
    {
        private readonly List<Error> _errors;

        public void ThrowErrors()
        {
            if(_errors.Count>0)
                throw new InvalidSyntaxException(_errors.ToArray());
        }

        public ErrorCollector()
        {
            _errors = new List<Error>();
        }

        public void SemErr(int line, int col, string s)
        {
            _errors.Add(new Error(line, col, s, ErrorType.Semantic));
        }

        public void SemErr(string s)
        {
            _errors.Add(new Error(-1, -1, s, ErrorType.Semantic));
        }

        public void SynErr(int line, int col, int n)
        {

		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "string expected"; break;
			case 4: s = "stringBq expected"; break;
			case 5: s = "internalMultiplicativeOp expected"; break;
			case 6: s = "internalAdditiveOp expected"; break;
			case 7: s = "compoundAssignmentOp expected"; break;
			case 8: s = "logicalOp expected"; break;
			case 9: s = "logicalBinaryOp expected"; break;
			case 10: s = "\"var\" expected"; break;
			case 11: s = "\",\" expected"; break;
			case 12: s = "\";\" expected"; break;
			case 13: s = "\"=\" expected"; break;
			case 14: s = "\"define\" expected"; break;
			case 15: s = "\"(\" expected"; break;
			case 16: s = "\")\" expected"; break;
			case 17: s = "\"function\" expected"; break;
			case 18: s = "\"{\" expected"; break;
			case 19: s = "\"}\" expected"; break;
			case 20: s = "\"true\" expected"; break;
			case 21: s = "\"false\" expected"; break;
			case 22: s = "\"++\" expected"; break;
			case 23: s = "\"--\" expected"; break;
			case 24: s = "\"while\" expected"; break;
			case 25: s = "\"do\" expected"; break;
			case 26: s = "\"break\" expected"; break;
			case 27: s = "\"if\" expected"; break;
			case 28: s = "\"else\" expected"; break;
			case 29: s = "\"return\" expected"; break;
			case 30: s = "??? expected"; break;
			case 31: s = "invalid Const"; break;
			case 32: s = "invalid OperandExpression"; break;
			case 33: s = "invalid OperandExpression"; break;
			case 34: s = "invalid TopLevelStatement"; break;
			case 35: s = "invalid Statement"; break;
			case 36: s = "invalid Statement"; break;
			case 37: s = "invalid LogicalExpression"; break;
			case 38: s = "invalid LogicalComparison"; break;

			default: s = "error " + n; break;
		}
            _errors.Add(new Error(line, col, s, ErrorType.Syntax));
        }

        public void Warning(int line, int col, string s)
        {
            _errors.Add(new Error(line, col, s, ErrorType.Syntax));
        }

        public void Warning(string s)
        {
            _errors.Add(new Error(-1, -1, s, ErrorType.Syntax));
        }

        public int Count => _errors.Count;

        public IEnumerator<Error> GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

[GeneratedCode("Coco/R","1.0.0")]
internal class FatalError: Exception {
	public FatalError(string m): base(m) {}
}

#endregion
}