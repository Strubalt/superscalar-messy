//package asm;

import java.util.*;


public class InstrEncoder {
	
	private class InstrType{
		
		public InstrType(int opCode, int type)
		{
			this.mOpCode = opCode;
			this.mType = type;
		}
		
		public int getType() {
			return this.mType;
		}
		public int getOpCode() {
			return this.mOpCode;
		}
		private int mType;
		private int mOpCode;
	}
	
	// opcode Rj Ri Rk xxxx xxxx 
	private static final int OpOnly = 1;
	private static final int REGijk = 2;
	private static final int RegjOffset20 = 3;
	private static final int REGijImm12 = 4;
	private static final int RegijOffset8 = 5;
	private static final int RegjkOffset8 = 6;
	private static final int Offset20 = 7;
	private static final int Offset24 = 8;
	private static final int Regj = 9;
	private static final int Imm32 = 10;
	private static final int REGjImm20 = 11;
	
	private HashMap opTable = new HashMap();
	
	public InstrEncoder(){
		opTable.put("NOP", 		new InstrType(0x00, OpOnly));
		opTable.put("ENABLE", 	new InstrType(0x23, OpOnly));
		opTable.put("DISABLE", 	new InstrType(0x24, OpOnly));
		opTable.put("SWI", 		new InstrType(0x25, OpOnly));
		opTable.put("HALT", 	new InstrType(0x26, OpOnly));
		opTable.put("RESUME", 	new InstrType(0x27, OpOnly));
		opTable.put("RETURN", 	new InstrType(0x28, OpOnly));
		
		opTable.put("ADD",  new InstrType(0x01, REGijk));
		opTable.put("SUB",  new InstrType(0x02, REGijk));
		opTable.put("MUL",  new InstrType(0x03, REGijk));
		opTable.put("DIV",  new InstrType(0x04, REGijk));
		opTable.put("AND",  new InstrType(0x05, REGijk));
		opTable.put("OR",   new InstrType(0x06, REGijk));
		opTable.put("XOR",  new InstrType(0x07, REGijk));
		opTable.put("NAND", new InstrType(0x08, REGijk));
		opTable.put("NOR",  new InstrType(0x09, REGijk));
		opTable.put("NXOR", new InstrType(0x0A, REGijk));
		
		opTable.put("MOVI", new InstrType(0x0B, REGjImm20));
		opTable.put("BEQZ", new InstrType(0x1B, RegjOffset20));
		opTable.put("BNEZ", new InstrType(0x1C, RegjOffset20));
		opTable.put("BLTZ", new InstrType(0x1D, RegjOffset20));
		opTable.put("BGEZ", new InstrType(0x1E, RegjOffset20));
		
		opTable.put("ADDI",  new InstrType(0x0F, REGijImm12));
		opTable.put("SUBI",  new InstrType(0x10, REGijImm12));
		opTable.put("MULI",  new InstrType(0x11, REGijImm12));
		opTable.put("DIVI",  new InstrType(0x12, REGijImm12));
		opTable.put("ANDI",  new InstrType(0x13, REGijImm12));
		opTable.put("ORI",   new InstrType(0x14, REGijImm12));
		opTable.put("XORI",  new InstrType(0x15, REGijImm12));
		opTable.put("NANDI", new InstrType(0x16, REGijImm12));
		opTable.put("NORI",  new InstrType(0x17, REGijImm12));
		opTable.put("NXORI", new InstrType(0x18, REGijImm12));
		
		opTable.put("LOAD", new InstrType(0x19, RegijOffset8));
		
		opTable.put("STORE", new InstrType(0x1A, RegjkOffset8));
		opTable.put("SWAP",  new InstrType(0x29, RegjkOffset8));
		
		opTable.put("BR", new InstrType(0x1F, Offset20));
		
		opTable.put("BLR", new InstrType(0x21, Offset24));
		
		opTable.put("BL", new InstrType(0x22, Regj));
		opTable.put("B",  new InstrType(0x20, Regj));
		
		opTable.put("DATA", new InstrType(0x00, Imm32));
	}
	
	byte[] encode(Instruction instr, int instrIndex, 
			HashMap symbolTable, HashMap constTable){
		byte data[] = new byte[4];
		int Ri, Rj, Rk;
		int imm, temp, offset;
		InstrType it = (InstrType)opTable.get(instr.operator());
		//System.out.print(it.getOpCode());
		//System.out.print(" ");
		//System.out.println(instr.operator());
		switch(it.getType()){
		case Imm32:
			imm = getImmediate(instr.token(1), symbolTable, constTable);
			data[0] = getByteValue(imm, 0);
			data[1] = getByteValue(imm, 1);
			data[2] = getByteValue(imm, 2);
			data[3] = getByteValue(imm, 3);
			break;
		case Regj:	//op   Rj
			Rj = getRegNum(instr.token(1));
			data[0] = 0;
			data[1] = 0;
			data[2] = getRjRiatByte2(Rj, 0);
			data[3] = getMSByte(it.getOpCode(), Rj);
			
			break;
		case Offset24:	//1000 0100 xxxx xxxx xxxx xxxx xxxx xxxx
						//BLR   offset
			offset = getOffset(instr.token(1), instrIndex, symbolTable, constTable);
			data[0] = getByteValue(offset, 0);
			data[1] = getByteValue(offset, 1);
			data[2] = getByteValue(offset, 2);
			data[3] = getMSByte(it.getOpCode(), 0);
			break;
		case Offset20:	//opcode 00 0000 xxxx xxxx xxxx xxxx xxxx
			offset = getOffset(instr.token(1), instrIndex, symbolTable, constTable);
			data[0] = getByteValue(offset, 0);
			data[1] = getByteValue(offset, 1);
			temp = (offset & 0xF0000);
			data[2] = (byte)(((offset & 0xF0000) >> 16) & 0xFF);
			data[3] = getMSByte(it.getOpCode(), 0);
			break;
		case RegjkOffset8:	//0110 10jj jjjj 0000 00kk kkkk xxxx xxxx
							//OP Rk,Rj,offset
			offset = getOffset(instr.token(3), instrIndex, symbolTable, constTable);
			Rj = getRegNum(instr.token(2));
			Rk = getRegNum(instr.token(1));
			data[0] = getByteValue(offset, 0);
			data[1] = (byte)Rk;
			data[2] = getRjRiatByte2(Rj, 0);
			data[3] = getMSByte(it.getOpCode(), Rj);
			break;
		case RegijOffset8:  //0110 01jj jjjj iiii ii00 0000 xxxx xxxx
							//LOAD  Ri,Rj,offset
			offset = getOffset(instr.token(3), instrIndex, symbolTable, constTable);
			Ri = getRegNum(instr.token(1));
			Rj = getRegNum(instr.token(2));
			data[0] = getByteValue(offset, 0);
			data[1] = (byte)((Ri << 6) & 0xFF);
			data[2] = data[2] = getRjRiatByte2(Rj, Ri);
			data[3] = getMSByte(it.getOpCode(), Rj);
			break;
		case REGijImm12:	//opcode jj jjjj iiii ii00 xxxx xxxx xxxx
							//op  Ri,Rj,immediate
			Ri = getRegNum(instr.token(1));
			Rj = getRegNum(instr.token(2));
			imm = getImmediate(instr.token(3), symbolTable, constTable);
			
			data[0] = getByteValue(imm, 0);
			temp = imm & 0xF00;
			data[1] = (byte)((Ri << 6 | temp >> 8) & 0xFF);
			data[2] = data[2] = getRjRiatByte2(Rj, Ri);
			data[3] = getMSByte(it.getOpCode(), Rj);
			break;
		case RegjOffset20: case REGjImm20: //opcode jj jjjj xxxx xxxx xxxx xxxx xxxx
						 //op  Rj,offset
			Rj = getRegNum(instr.token(1));
			//offset = Integer.decode(instr.token(2));
			if(it.getType() == REGjImm20)
				offset = getImmediate(instr.token(2), symbolTable, constTable);
			else
				offset = getOffset(instr.token(2), instrIndex, symbolTable, constTable);
			data[0] = getByteValue(offset, 0);
			data[1] = getByteValue(offset, 1);
			temp = offset & 0xF0000;
			data[2] = (byte)((Rj << 4 | temp >> 16) & 0xFF);
			data[3] = getMSByte(it.getOpCode(), Rj);
			break;
		case REGijk:  // opcode jj jjjj iiii iikk kkkk 0000 0000 
					  // op Ri, Rj, Rk
			Ri = getRegNum(instr.token(1));
			Rj = getRegNum(instr.token(2));
			Rk = getRegNum(instr.token(3));
			data[0] = 0;
			data[1] = (byte)((Ri << 6 | Rk ) & 0xFF);
			data[2] = getRjRiatByte2(Rj, Ri);
			data[3] = getMSByte(it.getOpCode(), Rj);
			break;
		case OpOnly:
			data[0] = data[1] = data[2] = 0;
			data[3] = getMSByte(it.getOpCode(), 0);
			break;
		}
		
		return data;
	}
	
	private byte getByteValue(int imm, int idx) {
		switch(idx){
		case 0:
			return (byte)((imm & 0xFF));
		case 1:
			return (byte)((imm & 0xFF00) >> 8);
		case 2:
			return (byte)((imm & 0xFF0000) >> 16);
		case 3:
			return (byte)((imm & 0xFF000000) >> 24);
		default:
				return-1;
		}
	}
	
	private byte getRjRiatByte2(int Rj, int Ri) {
		// jj_ jjjj iiii _ii
		return (byte)((Rj << 4 | Ri >> 2) & 0xFF);
	}
	
	private int getImmediate(String imm, HashMap symbolTable, HashMap constTable){
		
		
		try {
			int temp;
			temp = Integer.decode(imm).intValue();
			return temp;
		} catch(NumberFormatException e) {
			
		}
		if(constTable.containsKey(imm)) {
			Integer idx = (Integer)constTable.get(imm);
			return idx.intValue();
		}
		if(symbolTable.containsKey(imm)) {
			Integer idx = (Integer)symbolTable.get(imm);
			return idx.intValue() * 4;
		}
		System.out.println("Symbol no found:" + imm);
		throw new NumberFormatException();
		// Symbol
		
	}

	private int getOffset(String offset, int instrIndex, 
			HashMap symbolTable, HashMap constTable){
		
		
		try {
			int temp;
			temp = Integer.decode(offset).intValue();
			return temp;
		} catch(NumberFormatException e) {
			
		}
		if(constTable.containsKey(offset)) {
			Integer idx = (Integer)constTable.get(offset);
			return idx.intValue();
		}
		if(symbolTable.containsKey(offset)) {
			Integer idx = (Integer)symbolTable.get(offset);
			return (idx.intValue() - instrIndex) * 4;
		}
		System.out.println("Symbol no found:" + offset);
		throw new NumberFormatException();
		
	}
	
	private byte getMSByte(int opCode, int regNum) {
		return (byte)((opCode << 2 | regNum >> 4) & 0xFF);
	}
	
	private int getRegNum(String reg){
		if(Character.isDigit(reg.charAt(0))){
			return Integer.decode(reg).intValue();
		} else if(reg.charAt(0) == 'R'){
			return Integer.decode(reg.substring(1)).intValue();
		} else {
			if(reg.equals("LR"))
				return 53;
			if(reg.equals("LRI"))
				return 52;
			if(reg.equals("SP"))
				return 51;
			if(reg.equals("GP"))
				return 50;
		}
		return -1;
	}
	
	
}
