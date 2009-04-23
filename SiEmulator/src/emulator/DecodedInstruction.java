/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;
import java.util.*;
/**
 *
 * @author ys8511
 */
public class DecodedInstruction {

    static OpCodeTable table;
    
    private int opType;
    private int opCode;
    private int regi;
    private int regj;
    private int regk;
    private int immediate;
    
    public int instructionAddress;
    public int numCycleToFinish = 1;
    public int getOpType() { return this.opType; }
    public int getOpCode() { return this.opCode; }
    public int getRegi() { return regi; }
    public int getRegj() { return regj; }
    public int getRegk() { return regk; }
    public int getImmediate() { return immediate; }
    
    public DecodedInstruction(int instruction, int pc) {
        if(table == null) {
            table = new OpCodeTable();
        }
        instructionAddress = pc;
        opCode = (instruction >> 26) & 0x3F;
        //System.out.println(Integer.toBinaryString(instruction));
        //System.out.println(opCode);
        opType = table.getOpType(opCode);
        regj = (instruction >> 20) & 0x3F;
        regi = (instruction >> 14) & 0x3F;
        regk = (instruction >> 8) & 0x3F;
        immediate = decodeImmediate(instruction, opType);
        setNumCycleFinish();
    }
    
    private void setNumCycleFinish() {
        switch(opType) {
            case OpCodeTable.DIV: case OpCodeTable.DIVI:
                numCycleToFinish = Config.numDIVCycle;
                break;
            case OpCodeTable.MUL: case OpCodeTable.MULI:
                numCycleToFinish = Config.numMulCycle;
                break;
            
            default:
                numCycleToFinish = Config.numBasicCycle;
                break;
        }
    }
    
    
    //not include SWI and RESUME
    public boolean isBranch() {
        switch(getOpCode()) {
            case OpCodeTable.B: case OpCodeTable.BEQZ:
            case OpCodeTable.BGEZ: case OpCodeTable.BL:
            case OpCodeTable.BNEZ: case OpCodeTable.BLTZ:
            case OpCodeTable.BLR: case OpCodeTable.BR:
            case OpCodeTable.RETURN: 
                return true;
            default:
                return false;
        }
        
    }
            
    private int decodeImmediate(int instruction, int opType) {
        int result = 0;
        switch(opType) {
            case OpCodeTable.TP_REGj_OFFSET20: 
            case OpCodeTable.TP_OFFSET20: 
            case OpCodeTable.TP_REGj_IMM20:
                result = signExtend(instruction & 0xFFFFF, 19);
                break;
            case OpCodeTable.TP_REGij_IMM12:
                result = signExtend(instruction & 0xFFF, 11);
                break;
            case OpCodeTable.TP_REGij_OFFSET8: 
            case OpCodeTable.TP_REGjk_OFFSET8:
                result = signExtend(instruction & 0xFF, 7);
                break;
            case OpCodeTable.TP_OFFSET24:
                result = signExtend(instruction & 0xFFFFFF, 23);
                break;
            default:
                result = 0;
                break;
        }
        return result;
    }
    
    /**
	 * 
	 * @param signBitPosition - starting from 0
	 */
    private int signExtend(int value, int signBitPosition) {
        if(0 != (value & (1 << signBitPosition))) {
            return value | (0x80000000 >> (32-signBitPosition-2));
        }
        return value;
    }
    


}