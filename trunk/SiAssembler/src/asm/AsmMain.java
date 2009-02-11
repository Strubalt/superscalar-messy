package asm;

import java.io.*;

public class AsmMain {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		String inFile = "hello2.asm";
		String outFile = "hello2.out";
		
		Asm.assemble(inFile, outFile);
/*
		if(args.length > 1) {
			inFile = args[1];
			outFile = inFile;
			if(inFile.contains(".")){
				int idx = inFile.indexOf(".");
				if(inFile.substring(idx) != "out"){
					outFile = inFile.substring(0, idx);
				}
			}
			outFile += ".out";
			//Asm.assemble(inFile, outFile);
		}
		*/
	}

}
	
