
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;

namespace Ic10Transpiler.Parser {
#region COCO/R Generated Code

[GeneratedCode("Coco/R","1.0.0")]
internal class Token {
	public int kind;    // token kind
	public int pos;     // token position in bytes in the source text (starting at 0)
	public int charPos;  // token position in characters in the source text (starting at 0)
	public int col;     // token column (starting at 1)
	public int line;    // token line (starting at 1)
	public string val;  // token value
	public Token next;  // ML 2005-03-11 Tokens are kept in linked list
}

//-----------------------------------------------------------------------------------
// Buffer
//-----------------------------------------------------------------------------------
[GeneratedCode("Coco/R","1.0.0")]
internal interface IBuffer{
  int Read(); 
  int Peek();
  int Pos{get;set;}
  string GetString(int start, int end);
}
[GeneratedCode("Coco/R","1.0.0")]
static class Buffer{
  public const int EOF = char.MaxValue + 1;
}
//-----------------------------------------------------------------------------------
// Scanner
//-----------------------------------------------------------------------------------
[GeneratedCode("Coco/R","1.0.0")]
internal class Scanner {
	const char EOL = '\n';
	const int eofSym = 0; /* pdt */
	const int maxT = 31;
	const int noSym = 31;


	public IBuffer buffer; // scanner buffer
	
	Token t;          // current token
	int ch;           // current input character
	int pos;          // byte position of current character
	int charPos;      // position by unicode characters starting with 0
	int col;          // column number of current character
	int line;         // line number of current character
	int oldEols;      // EOLs that appeared in a comment;
	static readonly Dictionary<int,int> start; // maps first token character to start state

	Token tokens;     // list of tokens already peeked (first token is a dummy)
	Token pt;         // current peek token
	
	char[] tval = new char[128]; // text of current token
	int tlen;         // length of current token
	
	static Scanner() {
		start = new Dictionary<int,int>(128);
		for (int i = 36; i <= 36; ++i) start[i] = 1;
		for (int i = 65; i <= 90; ++i) start[i] = 1;
		for (int i = 95; i <= 95; ++i) start[i] = 1;
		for (int i = 97; i <= 122; ++i) start[i] = 1;
		for (int i = 48; i <= 57; ++i) start[i] = 14;
		for (int i = 34; i <= 34; ++i) start[i] = 4;
		for (int i = 39; i <= 39; ++i) start[i] = 6;
		for (int i = 45; i <= 45; ++i) start[i] = 30;
		start[46] = 2; 
		start[42] = 15; 
		start[47] = 16; 
		start[37] = 17; 
		start[43] = 31; 
		start[60] = 18; 
		start[62] = 19; 
		start[61] = 32; 
		start[33] = 9; 
		start[38] = 11; 
		start[124] = 12; 
		start[44] = 22; 
		start[59] = 23; 
		start[40] = 24; 
		start[41] = 25; 
		start[123] = 26; 
		start[125] = 27; 
		start[Buffer.EOF] = -1;

	}	
	
	public Scanner (IBuffer buffer) {
		this.buffer = buffer;
		Init();
	}
	
	void Init() {
		pos = -1; line = 1; col = 0; charPos = -1;
		oldEols = 0;
		NextCh();
		pt = tokens = new Token();  // first token is a dummy
	}
	
	void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; } 
		else {
			pos = buffer.Pos;
			// buffer reads unicode chars, if UTF8 has been detected
			ch = buffer.Read(); col++; charPos++;
			// replace isolated '\r' by '\n' in order to make
			// eol handling uniform across Windows, Unix and Mac
			if (ch == '\r' && buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; col = 0; }
		}

	}

	void AddCh() {
		if (tlen >= tval.Length) {
			char[] newBuf = new char[2 * tval.Length];
			Array.Copy(tval, 0, newBuf, 0, tval.Length);
			tval = newBuf;
		}
		if (ch != Buffer.EOF) {
			tval[tlen++] = (char) ch;
			NextCh();
		}
	}



	bool Comment0() {
		int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
		NextCh();
		if (ch == '*') {
			NextCh();
			for(;;) {
				if (ch == '*') {
					NextCh();
					if (ch == '/') {
						level--;
						if (level == 0) { oldEols = line - line0; NextCh(); return true; }
						NextCh();
					}
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			buffer.Pos = pos0; NextCh(); line = line0; col = col0; charPos = charPos0;
		}
		return false;
	}

	bool Comment1() {
		int level = 1, pos0 = pos, line0 = line, col0 = col, charPos0 = charPos;
		NextCh();
		if (ch == '/') {
			NextCh();
			for(;;) {
				if (ch == 10) {
					level--;
					if (level == 0) { oldEols = line - line0; NextCh(); return true; }
					NextCh();
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			buffer.Pos = pos0; NextCh(); line = line0; col = col0; charPos = charPos0;
		}
		return false;
	}


	void CheckLiteral() {
		switch (t.val) {
			case "var": t.kind = 11; break;
			case "define": t.kind = 15; break;
			case "function": t.kind = 18; break;
			case "true": t.kind = 21; break;
			case "false": t.kind = 22; break;
			case "while": t.kind = 25; break;
			case "do": t.kind = 26; break;
			case "break": t.kind = 27; break;
			case "if": t.kind = 28; break;
			case "else": t.kind = 29; break;
			case "return": t.kind = 30; break;
			default: break;
		}
	}

	Token NextToken() {
		while (ch == ' ' ||
			ch >= 9 && ch <= 10 || ch == 13 || ch == ' '
		) NextCh();
		if (ch == '/' && Comment0() ||ch == '/' && Comment1()) return NextToken();
		int recKind = noSym;
		int recEnd = pos;
		t = new Token();
		t.pos = pos; t.col = col; t.line = line; t.charPos = charPos;
		int state;
		if (start.ContainsKey(ch)) { state = (int) start[ch]; }
		else { state = 0; }
		tlen = 0; AddCh();
		
		switch (state) {
			case -1: { t.kind = eofSym; break; } // NextCh already done
			case 0: {
				if (recKind != noSym) {
					tlen = recEnd - t.pos;
					SetScannerBehindT();
				}
				t.kind = recKind; break;
			} // NextCh already done
			case 1:
				recEnd = pos; recKind = 1;
				if (ch == '$' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z') {AddCh(); goto case 1;}
				else {t.kind = 1; t.val = new String(tval, 0, tlen); CheckLiteral(); return t;}
			case 2:
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else {goto case 0;}
			case 3:
				recEnd = pos; recKind = 2;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 3;}
				else {t.kind = 2; break;}
			case 4:
				if (ch == '"') {AddCh(); goto case 5;}
				else if (ch <= '!' || ch >= '#' && ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 65535) {AddCh(); goto case 4;}
				else if (ch == 92) {AddCh(); goto case 20;}
				else {goto case 0;}
			case 5:
				{t.kind = 3; break;}
			case 6:
				if (ch == 39) {AddCh(); goto case 7;}
				else if (ch <= '!' || ch >= '#' && ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 65535) {AddCh(); goto case 6;}
				else if (ch == 92) {AddCh(); goto case 21;}
				else {goto case 0;}
			case 7:
				{t.kind = 4; break;}
			case 8:
				{t.kind = 8; break;}
			case 9:
				if (ch == '=') {AddCh(); goto case 10;}
				else {goto case 0;}
			case 10:
				{t.kind = 9; break;}
			case 11:
				if (ch == '&') {AddCh(); goto case 13;}
				else {goto case 0;}
			case 12:
				if (ch == '|') {AddCh(); goto case 13;}
				else {goto case 0;}
			case 13:
				{t.kind = 10; break;}
			case 14:
				recEnd = pos; recKind = 2;
				if (ch >= '0' && ch <= '9') {AddCh(); goto case 14;}
				else if (ch == '.') {AddCh(); goto case 2;}
				else {t.kind = 2; break;}
			case 15:
				recEnd = pos; recKind = 5;
				if (ch == '=') {AddCh(); goto case 8;}
				else {t.kind = 5; break;}
			case 16:
				recEnd = pos; recKind = 5;
				if (ch == '=') {AddCh(); goto case 8;}
				else {t.kind = 5; break;}
			case 17:
				recEnd = pos; recKind = 5;
				if (ch == '=') {AddCh(); goto case 8;}
				else {t.kind = 5; break;}
			case 18:
				recEnd = pos; recKind = 9;
				if (ch == '=') {AddCh(); goto case 10;}
				else {t.kind = 9; break;}
			case 19:
				recEnd = pos; recKind = 9;
				if (ch == '=') {AddCh(); goto case 10;}
				else {t.kind = 9; break;}
			case 20:
				if (ch == '"' || ch == 39 || ch == 92) {AddCh(); goto case 4;}
				else {goto case 0;}
			case 21:
				if (ch == '"' || ch == 39 || ch == 92) {AddCh(); goto case 6;}
				else {goto case 0;}
			case 22:
				{t.kind = 12; break;}
			case 23:
				{t.kind = 13; break;}
			case 24:
				{t.kind = 16; break;}
			case 25:
				{t.kind = 17; break;}
			case 26:
				{t.kind = 19; break;}
			case 27:
				{t.kind = 20; break;}
			case 28:
				{t.kind = 23; break;}
			case 29:
				{t.kind = 24; break;}
			case 30:
				recEnd = pos; recKind = 6;
				if (ch == '=') {AddCh(); goto case 8;}
				else if (ch == '-') {AddCh(); goto case 29;}
				else {t.kind = 6; break;}
			case 31:
				recEnd = pos; recKind = 7;
				if (ch == '=') {AddCh(); goto case 8;}
				else if (ch == '+') {AddCh(); goto case 28;}
				else {t.kind = 7; break;}
			case 32:
				recEnd = pos; recKind = 14;
				if (ch == '=') {AddCh(); goto case 10;}
				else {t.kind = 14; break;}

		}
		t.val = new String(tval, 0, tlen);
		return t;
	}
	
	private void SetScannerBehindT() {
		buffer.Pos = t.pos;
		NextCh();
		line = t.line; col = t.col; charPos = t.charPos;
		for (int i = 0; i < tlen; i++) NextCh();
	}
	
	// get the next token (possibly a token already seen during peeking)
	public Token Scan () {
		if (tokens.next == null) {
			return NextToken();
		} else {
			pt = tokens = tokens.next;
			return tokens;
		}
	}

	// peek for the next token, ignore pragmas
	public Token Peek () {
		do {
			if (pt.next == null) {
				pt.next = NextToken();
			}
			pt = pt.next;
		} while (pt.kind > maxT); // skip pragmas
	
		return pt;
	}

	// make sure that peeking starts at the current scan position
	public void ResetPeek () { pt = tokens; }

} // end Scanner
#endregion
}