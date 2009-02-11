package asm;



public class Instruction {
	
	public Instruction(){}
	
	public boolean isEmpty() {
		return this.mTokens.isEmpty();
	}
	private java.util.ArrayList<String> mTokens = new java.util.ArrayList<String>();
	void addToken(String t){
		this.mTokens.add(t);
	}
	
	public String command(){
		return mTokens.get(0);
	}
	
	public String argument(int idx){
		return mTokens.get(idx+1);
	}
	
}
