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

    boolean requireWriteBack;
    int     writeBackRegister;
    int     data;
    
    int     memoryAddress;
    boolean memoryRW;
    boolean memoryAccess;
    boolean memoryRWbar;
    
    
}
