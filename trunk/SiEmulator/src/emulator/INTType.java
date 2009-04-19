/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */

package emulator;

/**
 *
 * @author ys8511
 */
    public enum INTType {
        Timer(1), Interconnect(2), SWI(4), 
        PageFault(8), DividByZero(16), AccessPrivilegeReg(32);
        
        
        public int maskValue;
        INTType(int mask) {
            maskValue = mask;
        }
        
        public int setINT(int orgValue) {
            return orgValue | maskValue;
        }
        public int clearINT(int orgValue) {
            return orgValue & (~maskValue);
        }
    }
