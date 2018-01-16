using System.Collections.Generic;


using System;

namespace BluePrintAssembler.Steam {



internal class Parser {
	public const int _EOF = 0;
	public const int _stringValue = 1;
	public const int maxT = 4;

	const bool _T = true;
	const bool _x = false;
	const int minErrDist = 2;
	
	public Scanner scanner;
	public Errors  errors;

	public Token t;    // last recognized token
	public Token la;   // lookahead token
	int errDist = minErrDist;

internal string Name;
internal VDFObject Result;


	public Parser(Scanner scanner) {
		this.scanner = scanner;
		errors = new Errors();
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

	
	void Value(out VDFObject val) {
		val = null; 
		if (la.kind == 1) {
			Get();
			val = new VDFObject{ Value = t.val }; 
		} else if (la.kind == 2) {
			Get();
			PropList(out var list);
			val = new VDFObject{ Values = list }; 
			Expect(3);
		} else SynErr(5);
	}

	void PropList(out List<Tuple<string,VDFObject>> list ) {
		list = new List<Tuple<string,VDFObject>>(); 
		Prop(out var name1, out var val1);
		list.Add(Tuple.Create(name1,val1)); 
		if (la.kind == 1) {
			PropList(out var more);
			list.AddRange(more); 
		}
	}

	void Prop(out string name, out VDFObject val) {
		Expect(1);
		name = t.val; 
		Value(out val);
	}

	void VDF() {
		Prop(out var name, out var val);
		Name = name; Result = val; 
	}



	public void Parse() {
		la = new Token();
		la.val = "";		
		Get();
		VDF();
		Expect(0);

	}
	
	static readonly bool[,] set = {
		{_T,_x,_x,_x, _x,_x}

	};
} // end Parser


    // Errors
}