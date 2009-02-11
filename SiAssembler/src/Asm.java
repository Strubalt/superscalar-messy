//package asm;
import java.io.FileReader;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;



public class Asm {
	
	public static void assemble(String inFile, String outFile) throws IOException {
		ArrayList instrs = new ArrayList();
		HashMap symbolTable = new HashMap();
		HashMap constTable = new HashMap();
		
		buildTables(inFile, instrs, symbolTable, constTable);
		byte[] encodedResult = encode(instrs, symbolTable, constTable);
		
		writeFile(outFile, encodedResult);	
	}
	
	private static void writeFile(String outFile, byte[] data) throws IOException {
		if(data.length != 0){
			
			java.io.File f = new java.io.File(outFile);
			if(f.isFile()){
				if(!f.delete()){
					return;
				}
				if(!f.createNewFile()){
					return;
				}
			}
			
			java.io.FileOutputStream fw = null;
			try {
				fw = new java.io.FileOutputStream(f);
				fw.write(data);
			}finally {
				if(null != fw){
					fw.close();
				}
			}
		}
	}
	
	private static byte[] encode(
			ArrayList instrs, 
			HashMap symbolTable, 
			HashMap constTable){
		
		byte result[] = new byte[instrs.size()*4];
		byte data[];
		InstrEncoder encoder = new InstrEncoder();
		for(int i=0; i < instrs.size(); ++i) {
			data = encoder.encode((Instruction)instrs.get(i), i, symbolTable, constTable);
			for(int idx=0; idx<data.length; ++idx){
				result[i*4+idx] = data[idx];
			}
			String addr = Integer.toHexString(4*i);
			
			System.out.printf("0x%s", new Object[]{addr});
			System.out.print("\t");
			for(int j=data.length-1; j>=0; --j){
				int temp = data[j];
				if(temp<0) {
					temp = temp & 0X000000FF;
				}	
				String t = Integer.toHexString(temp);
				if(t.length()==1) t = "0" + t;
				System.out.print(t);
				System.out.print(" ");			
			}
			System.out.println();
	
		}
		return result;
		//return encodedResults;
	}
	
	private static void buildTables(String inFile, 
			ArrayList instrs, 
			HashMap symbolTable, 
			HashMap constTable) {
		
		//System.out.println(inFile);
		try {
			FileReader inStream = null;
			java.io.File f = new java.io.File(inFile);
			
			try {
				if(f.isFile()){
					inStream = new FileReader(inFile);
					buildTables(inStream, instrs, symbolTable, constTable);
				}
				
			} finally {
				if(null != inStream){
					inStream.close();
				}
			}
		} catch (IOException e) {
			
			e.printStackTrace();
			return;
		} 
		
	}
	
	private static void buildTables(FileReader inStream, 
			ArrayList instrs, 
			HashMap symbolTable, 
			HashMap constTable) throws IOException {
		
		
		int inChar = inStream.read();
		Instruction instr = new Instruction();
		String current = "";
		while(inChar != -1)
		{
			
			switch(inChar)
			{
				case '\r': case '\n':
					if(current.length() != 0) {
						instr.addToken(current);
						//System.out.println(current);
						current = "";
					}
					if(!instr.isEmpty()){
						instrs.add(instr);
						instr = new Instruction();
					}
					break;
				case ' ': case '\t':	//ignore whitespace
					if(current.length() != 0){
						instr.addToken(current);
						//System.out.println(current);
						current = "";
					}
					break;
				case '=':		//Constant
					Integer value = getInteger(inStream);
					//System.out.println(current + '=' + value.toString());
					
					constTable.put(current, value);
					current = "";
					break;
				case ':':		//Label
					symbolTable.put(current, new Integer(instrs.size()));
					//System.out.print(current + '=' );
					//System.out.println(instrs.size());
					
					current = "";
					break;
				case '#':
					skipLine(inStream);
					break;
				case '\'':		//Character
					Integer ch = getChar(inStream);
					//System.out.println(ch.toString());
					instr.addToken(ch.toString());
					break;
				case ',':
					instr.addToken(current);
					//System.out.println(current);
					current = "";
					break;
				default:
					java.lang.Character c = new Character(Character.toUpperCase((char)inChar));
					current = current.concat(c.toString());
					break;
			}
			inChar = inStream.read();
		}
		
	}
	
	private static void skipLine(FileReader inStream){
		int inChar;
		//String ret = "";
		try{
			inChar = inStream.read();
			while(inChar != -1) {
				if(inChar == '\n' || inChar == '\r'){
					break;
				}
				inChar = inStream.read();
			}
			
		} catch (IOException e) {
			System.out.println(e.toString());
		}
	}
	
	private static Integer getChar(FileReader inStream) {
		int inChar;
		String ret = "";
		boolean found = false;
		try{
			inChar = inStream.read();
			
			while(inChar != -1) {
				switch(inChar)
				{
				case '\'':
					found = true;
					break;
				
				default:
					java.lang.Character c = new Character((char)inChar);
					if(ret.length()==0) 
						ret = c.toString();
					else if (ret.charAt(0)=='\\'){
						switch(inChar){
						case 't': 
							ret = "\t";
							break;
						case 'n':
							ret = "\n";
							break;
						case 'b':
							ret = "\b";
							break;
						case 'f':
							ret = "\f";
							break;
						case 'r':
							ret = "\r";
							break;
						case '\"':
							ret = "\"";
							break;
						case '\'':
							ret = "\'";
							break;
						case '\\':
							ret = "\\";
							break;
						}
					}
				}
				if(found) break;
				inChar = inStream.read();
			}
			
		} catch (IOException e) {
			System.out.println(e.toString());
		}
		return new Integer(ret.charAt(0));
	}
	
	private static Integer getInteger(FileReader inStream) throws NumberFormatException  {
		int inChar;
		String ret = "";
		try{
			inChar = inStream.read();
			while(inChar != -1) {
				if(inChar == '\t' || inChar == ' ')	{
				}
				else { 
					break; 
				}
				inChar = inStream.read();
			}
			while(inChar != -1) {
				if(inChar == '\r' || inChar == '\n')
				{
					break;
				}
				ret += (char)inChar;
				inChar = inStream.read();
			}
			
		} catch (IOException e) {
			System.out.println(e.toString());
		}
		
		return Integer.decode(ret);
		
	}
	
}
