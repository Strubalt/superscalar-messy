/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;

/**
 *
 * @author ys8511
 */
public class ExecutedInstruction {
    
    private DecodedInstruction instr;
    
    public ExecutedInstruction(DecodedInstruction instruction) {
        this.instr = instruction;
    }
    
    public int instrAddress() {
        return instr.instructionAddress;
    }

    boolean hasException;
    boolean requireWriteBack;
    INTType exceptionType;
    
    int     targetRegister;
    //int     data;
    int     regData;
    int     memData;
    int     memoryAddress;
    boolean memoryReadThenWrite;
    boolean memoryAccess;
    boolean memoryRWbar;
    
    boolean isHalt;
    
    boolean requireModifyPC;
    int newPC;
    
}