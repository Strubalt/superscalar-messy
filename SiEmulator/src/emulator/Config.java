/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;

import java.io.*;
/**
 *
 * @author ys8511
 */
public class Config {
    
    
    
    
   
    static final String PIPELINE="PIPELINE";
    static final String CACHE_NUM_SET="CACHE_NUM_SET";
    static final String CACHE_NUM_WAY="CACHE_NUM_WAY";

    static final String DIV_CYCLE="DIV_CYCLE";
    static final String MUL_CYCLE="MUL_CYCLE";
    static final String INSTR_CYCLE="INSTR_CYCLE";

    static final String MEM_SIZE_WORD="MEM_SIZE_WORD";
    static final String EX_CLK="EX_CLK";

    static final String TERM_BASE_ADDR="TERM_BASE_ADDR";
    static final String TERM_DELAY_AVG="TERM_DELAY_AVG";
    static final String TERM_BUFFER_SIZE="TERM_BUFFER_SIZE";
    static final String TERM_SHOW_OUT_DELAY="TERM_SHOW_OUT_DELAY";
    static final String TERM_SHOW_IN_DELAY="TERM_SHOW_IN_DELAY";
    
    
    static boolean pipeline = true;
    static int cacheNumSet = 16, cacheNumWay = 4;
    static int numDIVCycle = 5, numMulCycle = 3, numBasicCycle = 1;
    static int MemorySizeWord = 900;
    static int ExCLK = 5;
    static int TermBaseAddr = 0xf00;
    static int TermDelayAvg = 100;
    static int TermBufferSize = 200;
    static boolean TermShowInDelay = false, TermShowOutDelay = false;
    
    private static void setValue(String fieldName, int fieldValue) {
        if(fieldName.equals(PIPELINE)) {
            pipeline = (fieldValue != 0);
        } else if(fieldName.equals(CACHE_NUM_SET)) {
            cacheNumSet = fieldValue;
        } else if(fieldName.equals(CACHE_NUM_WAY)) {
            cacheNumWay = fieldValue;
        } else if(fieldName.equals(DIV_CYCLE)) {
            numDIVCycle = fieldValue;
        } else if(fieldName.equals(MUL_CYCLE)) {
            numMulCycle = fieldValue;
        } else if(fieldName.equals(INSTR_CYCLE)) {
            numBasicCycle = fieldValue;
        } else if(fieldName.equals(MEM_SIZE_WORD)) {
            MemorySizeWord = fieldValue;
        } else if(fieldName.equals(EX_CLK)) {
            ExCLK = fieldValue;
        } else if(fieldName.equals(TERM_BASE_ADDR)) {
            TermBaseAddr = fieldValue;
        } else if(fieldName.equals(TERM_DELAY_AVG)) {
            TermDelayAvg = fieldValue;
        } else if(fieldName.equals(TERM_BUFFER_SIZE)) {
            TermBufferSize = fieldValue;
        } else if(fieldName.equals(TERM_SHOW_OUT_DELAY)) {
            TermShowOutDelay = (fieldValue != 0);
        } else if(fieldName.equals(TERM_SHOW_IN_DELAY)) {
            TermShowInDelay = (fieldValue != 0);
        } else {
            assert(true);
        } 
    
    }
    
    static void Load(String fileName) {
        java.io.File f = new java.io.File(fileName);
        if(f.isFile()){
            //System.out.println("Load Config file");
            String current;
            try {
                    BufferedReader br = new BufferedReader(new FileReader(fileName));
                    while ((current = br.readLine()) != null) { // while loop begins here

                        String data[] = current.split("=");
                        if(data != null && data.length == 2) {
                            try{
                                int fieldValue = Integer.parseInt(data[1]);
                            
                                setValue(data[0], fieldValue);
                            } catch (Exception e) {
                                
                            }
                            
                        }
                        
                    } // end while 
            } // end try
            catch (IOException e) {
                    System.err.println("Error: " + e);
            }
        }
	TermDelayAvg /= ExCLK;	
		
    }
    
    
}
