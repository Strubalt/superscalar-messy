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

    
    public class OpCodeTable {
            
        public static final int TP_OP_ONLY = 1;
        public static final int TP_REGijk = 2;
        public static final int TP_REGj_OFFSET20 = 3;
        public static final int TP_REGij_IMM12 = 4;
        public static final int TP_REGij_OFFSET8 = 5;
        public static final int TP_REGjk_OFFSET8 = 6;
        public static final int TP_OFFSET20 = 7;
        public static final int TP_OFFSET24 = 8;
        public static final int TP_REGj = 9;
        public static final int TP_REGj_IMM20 = 10;
        
        private HashMap<Integer,Integer> map = new HashMap<Integer,Integer>();
        
        public OpCodeTable() {
            map.put(NOP, TP_OP_ONLY);
            map.put(ENABLE, TP_OP_ONLY);
            map.put(DISABLE, TP_OP_ONLY);
            map.put(SWI, TP_OP_ONLY);
            map.put(HALT, TP_OP_ONLY);
            map.put(RESUME, TP_OP_ONLY);
            map.put(RETURN, TP_OP_ONLY);
            
            map.put(ADD, TP_REGijk);
            map.put(SUB, TP_REGijk);
            map.put(MUL, TP_REGijk);
            map.put(DIV, TP_REGijk);
            map.put(AND, TP_REGijk);
            map.put(OR, TP_REGijk);
            map.put(XOR, TP_REGijk);
            map.put(NAND, TP_REGijk);
            map.put(NOR, TP_REGijk);
            map.put(NXOR, TP_REGijk);
            
            map.put(MOVI, TP_REGj_IMM20);
            
            map.put(BEQZ, TP_REGj_OFFSET20);
            map.put(BNEZ, TP_REGj_OFFSET20);
            map.put(BLTZ, TP_REGj_OFFSET20);
            map.put(BGEZ, TP_REGj_OFFSET20);
            
            map.put(ADDI, TP_REGij_IMM12);
            map.put(SUBI, TP_REGij_IMM12);
            map.put(MULI, TP_REGij_IMM12);
            map.put(DIVI, TP_REGij_IMM12);
            map.put(ANDI, TP_REGij_IMM12);
            map.put(ORI, TP_REGij_IMM12);
            map.put(XORI, TP_REGij_IMM12);
            map.put(NANDI, TP_REGij_IMM12);
            map.put(NORI, TP_REGij_IMM12);
            map.put(NXORI, TP_REGij_IMM12);
            
            map.put(LOAD, TP_REGij_OFFSET8);
            
            map.put(STORE, TP_REGjk_OFFSET8);
            map.put(SWAP, TP_REGjk_OFFSET8);
            
            map.put(BR, TP_OFFSET20);
            
            map.put(BLR, TP_OFFSET24);
            
            map.put(BL, TP_REGj);
            map.put(B, TP_REGj);
        }
        
        public int getOpType(int opCode) {
             return map.get(opCode);
             /*
            try {
                System.out.print(opCode + ",");
                return map.get(opCode);
            } catch(Exception e) {
                System.out.println("error");
                return -1;
            } finally {
                
            }
            */
        }

        //OpOnly
        public static final int NOP = 0x00;
        public static final int ENABLE = 0x23;
        public static final int DISABLE = 0x24;
        public static final int SWI = 0x25;
        public static final int HALT = 0x26;
        public static final int RESUME = 0x27;
        public static final int RETURN = 0x28;
        
        //REGijk
        public static final int ADD = 0x01;
        public static final int SUB = 0x02;
        public static final int MUL = 0x03;
        public static final int DIV = 0x04;
        public static final int AND = 0x05;
        public static final int OR = 0x06;
        public static final int XOR = 0x07;
        public static final int NAND = 0x08;
        public static final int NOR = 0x09;
        public static final int NXOR = 0x0A;
    
        //REGjImm20
        public static final int MOVI = 0x0B;
        
        //RegjOffset20
        public static final int BEQZ = 0x1B;
        public static final int BNEZ = 0x1C;
        public static final int BLTZ = 0x1D;
        public static final int BGEZ = 0x1E;
                
        //REGijImm12
        public static final int ADDI = 0x0F;
        public static final int SUBI = 0x10;
        public static final int MULI = 0x11;
        public static final int DIVI = 0x12;
        public static final int ANDI = 0x13;
        public static final int ORI = 0x14;
        public static final int XORI = 0x15;
        public static final int NANDI = 0x16;
        public static final int NORI = 0x17;
        public static final int NXORI = 0x18;
        
        //RegijOffset8
        public static final int LOAD = 0x19;
        
        //RegjkOffset8
        public static final int STORE = 0x1A;
        public static final int SWAP = 0x29;
        
        //Offset20
        public static final int BR = 0x1F;
        
        //Offset24
        public static final int BLR = 0x21;
        
        //Regj
        public static final int BL = 0x22;
        public static final int B = 0x20;
    }

