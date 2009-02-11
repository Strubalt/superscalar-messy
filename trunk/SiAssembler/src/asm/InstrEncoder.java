package asm;

import java.util.*;

public class InstrEncoder {
	
	private static final int REG3 = 1;
	
	
	private HashMap<String, Integer> opcodes = new HashMap<String, Integer>();
	
	public InstrEncoder(){
		opcodes.put("NOP", 0x00);
		opcodes.put("ADD", 0x01);
		opcodes.put("SUB", 0x02);
		opcodes.put("MUL", 0x03);
		opcodes.put("DIV", 0x04);
		opcodes.put("AND", 0x05);
		opcodes.put("OR", 0x06);
		opcodes.put("XOR", 0x07);
		opcodes.put("NAND", 0x08);
		opcodes.put("NOR", 0x09);
		opcodes.put("NXOR", 0x0A);
		
		opcodes.put("MOVI", 0x0B);
		
		opcodes.put("ADDI", 0x0F);
		opcodes.put("SUBI", 0x10);
		opcodes.put("MULI", 0x11);
		opcodes.put("DIVI", 0x12);
		opcodes.put("ANDI", 0x13);
		opcodes.put("ORI", 0x14);
		opcodes.put("XORI", 0x15);
		opcodes.put("NANDI", 0x16);
		opcodes.put("NORI", 0x17);
		opcodes.put("NXORI", 0x18);
		
		opcodes.put("LOAD", 	0x19);
		opcodes.put("STORE", 	0x1A);
		opcodes.put("BEQZ", 	0x1B);
		opcodes.put("BNEZ", 	0x1C);
		opcodes.put("BLTZ", 	0x1D);
		opcodes.put("BGEZ", 	0x1E);
		opcodes.put("BR", 		0x1F);
		opcodes.put("B", 		0x20);
		opcodes.put("BLR", 		0x21);
		opcodes.put("BL", 		0x22);
		opcodes.put("ENABLE", 	0x23);
		opcodes.put("DISABLE", 	0x24);
		opcodes.put("SWI", 		0x25);
		opcodes.put("HALT", 	0x26);
		opcodes.put("RESUME", 	0x27);
		opcodes.put("RETURN", 	0x28);
		opcodes.put("SWAP", 	0x29);
		//opcodes.put("DATA", 0x);
	}
	
	byte[] encode(Instruction instr, HashMap<String, Integer> symbolTable){
		byte data[] = new byte[4];
		switch(instr.command()){
		case "ADD": case "SUB": case "MUL": case "DIV":
		case "AND": case "OR": case "XOR": case "NAND":
		case "NOR": case "NXOR":
			break;
			
		
			
		}
		String str1[] = {"NOP", "ENABLE", "DISABLE", "SWI", 
				"HALT", "RESUME", "RETURN", "SWAP"};
		
		String str2[] = {"ADD", "SUB", "MUL", "DIV", 
				"AND", "OR", "XOR", "NAND", "NOR", "NXOR"};
		String str3[] = {"MOVI", "BEQZ", "BNEZ", "BLTZ", "BGEZ"};
		String str4[] = {"ADDI", "SUBI", "MULI", "DIVI", 
				"ANDI", "ORI", "XORI", "NANDI", "NORI", "NXORI"};
		String str5[] = {"LOAD"};
		String str6[] = {"STORE", "SWAP"};
		String str7[] = {"BR"};
		String str8[] = {"BLR"};
		String str9[] = {"B", "BL"};
		String str10[] = {"DATA"};
		return data;
	}
	
	
	
}
