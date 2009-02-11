//package asm;



public class Instruction {
	
	public Instruction(){}
	
	public boolean isEmpty() {
		return this.mTokens.isEmpty();
	}
	private java.util.ArrayList mTokens = new java.util.ArrayList();
	void addToken(String t){
		this.mTokens.add(t);
	}
	
	public String operator(){
		return (String)mTokens.get(0);
	}
	
	public String token(int idx){
		return (String)mTokens.get(idx);
	}
	
	public String toString(){
		if(mTokens.size()==0) return "";
		String ret = (String)mTokens.get(0);
		for(int i=1; i<mTokens.size(); ++i){
			ret += " " + (String)mTokens.get(i);
		}
		return ret;
	}
	
}
