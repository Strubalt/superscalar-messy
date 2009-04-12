package emulator;
//import java.util.*;


class Processor extends Component {
	
    final static int REG_LR = 53;
    final static int REG_LRI = 52;
    final static int REG_SP = 51;
    final static int REG_GP = 50;
    final static int REG_STATUS = 57;
    final static int REG_INTERRUPT = 58;
    final static int REG_TIMER = 59;
    final static int REG_BASE = 60;
    final static int REG_LIMIT = 61;
    final static int REG_RING = 62;
    final static int REG_PID = 63;

    final static int STG_FETCH = 0;
    final static int STG_DECODE = 1;
    final static int STG_EXECUTE = 2;
    final static int STG_MEM = 3;
    final static int STG_WB = 4;
    //private ArrayList<Interconnect> interconnects = new ArrayList<Interconnect>();
    Interconnect bus;
    private int registers[];

    private int stage; //IF=0, DEC=1, EX, MEM, WB
   
    private int pc;
    private boolean isHalt; //need to make sure all instructions have finished
    private MMU mmu = new MMU();
    private DecodedInstruction fetchedInstruction;
    private DecodedInstruction decodedInstruction;
    private ExecutedInstruction executedInstruction;
    private ExecutedInstruction memAccessedInstruciton;
    private int cycle;
    //private boolean INTEnable;




    public Processor() {
            super(0);
            pc = 0;
            cycle = 0;
            stage = STG_FETCH;
            registers = new int[64];
            registers[REG_LIMIT]=0x7FFFFFFF;
    }


    void attachInterconnect(Interconnect connection) {
            // TODO Auto-generated method stub
            bus = connection;
            mmu.attachInterconnect(connection);
    }

    

    @Override
    void advanceTime() {
        switch(stage) {
            case STG_FETCH:

                if(hasInterrupt()) {
                    cycle += 1;
                    processHWI();
                } else {
                    if(fetchInstruction()) {
                        cycle += 1;
                        stage += 1;
                    }
                }

                break;
            case STG_DECODE:
                if(decode()) {
                    stage += 1;
                }
                break;                
            case STG_EXECUTE:
                if(execute()) {
                    stage += 1;
                }
                break;
            case STG_MEM:
                if(memoryAccess()) {
                    stage += 1;
                }
                break;
            case STG_WB:
                if(writeBack()) {
                    stage = STG_FETCH;
                }
                break;            
        }
    }
    
    private boolean INTEnable() {
        int reg = this.getRegSafe(REG_INTERRUPT);
        return ((reg >> 31) != 0);
    }
    
    private void enableInterrupt(boolean isEnable) {
        
        int reg = this.getRegSafe(REG_INTERRUPT);
        if(isEnable) {
            reg = reg | 0x80000000;
        } else {
            reg = reg & 0x7FFFFFFF;
        }
    }

    private boolean hasInterrupt() {
        if(!this.INTEnable()) return false;
        if(bus.isInterrupted) return true;
        return false;
    }

    private void processHWI() {
        
        this.setRegSafe(REG_LRI, pc);
        pc = 4;
        enableInterrupt(false);
    }

    private boolean fetch() {
        if(hasInterrupt()) {
            cycle += 1;
            processHWI();
        } else {
            if(fetchInstruction()) {
                return true;
            }
        }
        return false;
    }
    
    private boolean decode(){
        decodedInstruction = this.fetchedInstruction;
        this.fetchedInstruction = null;
        return true;
    }

    private boolean execute() {
        if(null != decodedInstruction) {
            executedInstruction = new ExecutedInstruction(decodedInstruction);
            execute(decodedInstruction, executedInstruction);
            decodedInstruction = null;
        }
        return true;
    }

    private void setResult(ExecutedInstruction result, 
            int targetReg, int data, boolean memoryAccess) {
        result.requireWriteBack = true;
        result.data = data;
        result.writeBackRegister = targetReg;
        result.memoryAccess = memoryAccess;
    }

    private void execute(DecodedInstruction instr, ExecutedInstruction result) {
        boolean pcAdd4 = true;
        switch(instr.getOpCode()){
            case OpCodeTable.ADD:   //Ri = Rj + Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())+getRegSafe(instr.getRegk()), false);
                break;
            case OpCodeTable.AND:   //Ri = Rj & Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) & getRegSafe(instr.getRegk()), false);
                break;
            case OpCodeTable.DIV:   //Ri = Rj / Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) / getRegSafe(instr.getRegk()), false);
                break;    
            case OpCodeTable.MUL:   //Ri = Rj * Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) * getRegSafe(instr.getRegk()), false);                    
                break;

            case OpCodeTable.NAND:  //Ri = ~( Rj & Rk )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) & getRegSafe(instr.getRegk())), false);
                break;
            case OpCodeTable.NXOR:  //Ri = ~( Rj ^ Rk )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) ^ getRegSafe(instr.getRegk())), false);
                break;
            case OpCodeTable.NOR:   //Ri = ~( Rj | Rk )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) | getRegSafe(instr.getRegk())), false);
                break;
            case OpCodeTable.OR:    //Ri = Rj | Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) | getRegSafe(instr.getRegk()), false);
                break;
            case OpCodeTable.XOR:   //Ri = Rj ^ Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) ^ getRegSafe(instr.getRegk()), false);
                break;
            case OpCodeTable.SUB:   //Ri = Rj - Rk
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj()) - getRegSafe(instr.getRegk()), false);
                break;
            case OpCodeTable.SUBI:  //Ri = Rj - x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())-instr.getImmediate(), false);
                break;
            case OpCodeTable.ADDI:  //Ri = Rj + x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())+instr.getImmediate(), false);
                break;
            case OpCodeTable.ANDI:  //Ri = Rj & x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())&instr.getImmediate(), false);
                break;
            case OpCodeTable.DIVI:  //Ri = Rj / x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())/instr.getImmediate(), false);
                break;
            case OpCodeTable.MULI:  //Ri = Rj * x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())*instr.getImmediate(), false);
                break;
            case OpCodeTable.NXORI: //Ri = ~( Rj ^ x )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) ^ instr.getImmediate()), false);
                break;
            case OpCodeTable.NANDI: //Ri = ~( Rj & x )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) & instr.getImmediate()), false);
                break;
            case OpCodeTable.XORI:  //Ri = Rj ^ x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())^instr.getImmediate(), false);
                break;
            case OpCodeTable.ORI:   //Ri = Rj | x
                setResult(result, instr.getRegi(), 
                    getRegSafe(instr.getRegj())|instr.getImmediate(), false);
                break;
            case OpCodeTable.NORI:  //Ri = ~( Rj | x )
                setResult(result, instr.getRegi(), 
                    ~(getRegSafe(instr.getRegj()) | instr.getImmediate()), false);
                break;
            case OpCodeTable.MOVI:  //Rj = immediate
                setResult(result, instr.getRegj(), 
                    instr.getImmediate(), false);
                break;



            case OpCodeTable.HALT:  //Stop execution & print registers
                result.isHalt = true;
                break;
            case OpCodeTable.LOAD:  //Ri=Mem[Rj+x]
                
                result.memoryAddress = getRegSafe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = true;
                result.memoryRW = false;
                
                setResult(result, instr.getRegi(), 0, true);
                break;
            case OpCodeTable.STORE: //Mem[Rj+x]=Rk
                result.memoryAccess = true;
                result.memoryAddress = getRegSafe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = false;
                result.memoryRW = false;
                result.data = getRegSafe(instr.getRegk());
                break;
            case OpCodeTable.SWAP:  //swap Mem[Rj+x] and Rk
                result.memoryAccess = true;
                result.memoryAddress = getRegSafe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = false;
                result.memoryRW = true;
                result.requireWriteBack = true;
                
                setResult(result, instr.getRegk(), getRegSafe(instr.getRegk()), true);
                break;
            case OpCodeTable.DISABLE: //Disable interrupts
                enableInterrupt(false);
                break;               
            case OpCodeTable.ENABLE://Enable interrupts
                enableInterrupt(true);
                break;    
            case OpCodeTable.SWI:   //ENABLE=0; LRI = PC; PC = 8
                                    //r8--r15 should be saved away.
                                    //Argument passed in r0
                enableInterrupt(false);
                setRegSafe(REG_LRI, pc + 4);
                pc = 8;
                saveReg8_15();
                pcAdd4 = false;
                break;
            case OpCodeTable.RESUME: //PC = LRI; ENABLE=1;
                                     //r8--r15 should be restored.
                                     //Result passed in r0
                enableInterrupt(true);
                pc = getRegSafe(REG_LRI);
                restoreReg8_15();
                pcAdd4 = false;
                break;
            case OpCodeTable.NOP:   //
                break;
            case OpCodeTable.B:     //PC = Rj
                pc = getRegSafe(instr.getRegj());
                pcAdd4=false;
                break;
            case OpCodeTable.BEQZ:  //IF Rj=0  THEN PC=PC+x
                if(getRegSafe(instr.getRegj()) == 0) {
                    pc += instr.getImmediate();
                    pcAdd4=false;
                }
                break;
            case OpCodeTable.BNEZ:  //IF Rj!=0 THEN PC=PC+x
                if(getRegSafe(instr.getRegj()) != 0) {
                    pc += instr.getImmediate();
                    pcAdd4=false;
                }
                break;
            case OpCodeTable.BGEZ:  //IF Rj>=0 THEN PC=PC+x
                if(getRegSafe(instr.getRegj()) >= 0) {
                    pc += instr.getImmediate();
                    pcAdd4=false;
                }
                break;
            case OpCodeTable.BLTZ:  //IF Rj<0  THEN PC=PC+x
                if(getRegSafe(instr.getRegj()) < 0) {
                    pc += instr.getImmediate();
                    pcAdd4=false;
                }
                break;

            case OpCodeTable.BLR:   //LR = PC; PC = PC+x
                setRegSafe(REG_LR, pc + 4);
                pc = pc + instr.getImmediate();
                pcAdd4 = false;
                break;
            case OpCodeTable.BR:    //PC = PC+x
                pc = pc + instr.getImmediate();
                pcAdd4 = false;
                break;
            case OpCodeTable.BL:    //LR = PC; PC = Rj
                setRegSafe(REG_LR, pc + 4);
                pc = getRegSafe(instr.getRegj());
                pcAdd4 = false;
                break;
            case OpCodeTable.RETURN: //PC = LR
                pc = getRegSafe(REG_LR);
                pcAdd4 = false;
                break;
        }
        if(pcAdd4) {
            pc+=4;
        }
    }

    private int temp[] = new int[8];
    private void saveReg8_15() {
        for(int i=0; i<8; ++i) {
            temp[i] = getRegSafe(8+i);
        }
    }

    private void restoreReg8_15() {
        for(int i=0; i<8; ++i) {
            setRegSafe(8+i, temp[i]);
        }
    }

    private boolean memoryAccess() {
        if(null != executedInstruction) {

            if(executedInstruction.memoryAccess) {
                if(!accessMemory()) {
                    return false;
                }
            }

        }
        memAccessedInstruciton = executedInstruction;
                executedInstruction = null;
        return true;
    }

    private boolean accessMemory() {
        if(executedInstruction.memoryRW) {
            mmu.readData(executedInstruction.memoryAddress);
            if(this.mmu.getDataReady()) {
                int orgData = executedInstruction.data;
                executedInstruction.data = this.mmu.getData();
                mmu.writeData(executedInstruction.memoryAddress, orgData);  
            } else {
                return false;
            }
        } else {
            if(executedInstruction.memoryRWbar) {
                mmu.readData(executedInstruction.memoryAddress);
                if(this.mmu.getDataReady()) {
                    executedInstruction.data = this.mmu.getData();
                } else {
                    return false;
                }
            } else {
                mmu.writeData(executedInstruction.memoryAddress, 
                              executedInstruction.data);  
            }    
        }
        return true;
    }

    private boolean writeBack() {
        if(null != memAccessedInstruciton) {
            if(memAccessedInstruciton.requireWriteBack) {

                setRegSafe(memAccessedInstruciton.writeBackRegister, 
                        memAccessedInstruciton.data);
            }
            
            
            if(memAccessedInstruciton.isHalt) {
                isHalt = true;
            }
            memAccessedInstruciton = null;
        }
        return true;
    }



    private boolean fetchInstruction(){
        this.mmu.readInstruction(this.pc);
        if(this.mmu.getInstrReady()) {
            //this.fetchedInstruction = this.mmu.getInstruction();
            fetchedInstruction = new DecodedInstruction(this.mmu.getInstruction());
            //System.out.println(Integer.toBinaryString(this.fetchedInstruction));

            return true;
        }
        return false;
    }


    public boolean isHalt() {
        return this.isHalt;
    }

    private int getRegSafe(int regNumber) {
        return registers[regNumber];
    }
    
    private void setRegSafe(int regNumber, int value) {
        registers[regNumber] = value;
    }
    
    //don't use, only for debugging
    public int getReg(int regNumber) {
        return registers[regNumber];
    }

    //don't use, only for debugging
    public void setReg(int regNumber, int value) {
        registers[regNumber] = value;
    }

    //only for debugging
    public int getRing() {
        return registers[REG_RING];
    }

    public void setRing(int ringLevel) {
            registers[REG_RING] = ringLevel;
    }

    public void setPC(int address) {
            this.pc = address;
    }

    public void executeOpcode(int instruction) {
        int op = getOp(instruction);
        int tempPC = pc;
        decodedInstruction = new DecodedInstruction(instruction);
        executedInstruction = new ExecutedInstruction(decodedInstruction);
        execute(decodedInstruction, executedInstruction);
        writeBack();
        decodedInstruction = null;
        executedInstruction = null;
        pc = tempPC;
    }

    void dumpRegs() {

            for(int i=0; i<this.registers.length; ++i) {
                    System.out.println("R" + new Integer(i).toString() + 
                                    " = " + new Integer(this.registers[i]).toString());
            }
            System.out.println("Current Cycle: " + Integer.toString(this.cycle));
    }

    private int getOp(int instruction) {
            return ((instruction >> 26) & 0x3F);
    }

}