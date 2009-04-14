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
   
    private int programCounter;
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
            programCounter = 0;
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

    void advanceTime_Basic() {
        cycle+=1;
        switch(stage) {
            case STG_FETCH:
                //cycle+=1;
                if(fetch()) {
                    stage += 1;
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
                if(executedInstruction.requireModifyPC)
                    programCounter = executedInstruction.newPC;
                else
                    programCounter += 4;
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
    
    void advanceTimePipeline() {
        cycle+=1;
        writeBack();
        memoryAccess();
        execute();
        decode();
        boolean next = fetch();
        if(null != executedInstruction && executedInstruction.requireModifyPC) {
            programCounter = executedInstruction.newPC;
        } else if(next) {
            programCounter += 4;
        }
    }

    @Override
    void advanceTime() {
        //this.advanceTime_Basic();
        this.advanceTimePipeline();
    }
    
    private boolean INTEnable() {
        int reg = this.getRegExe(REG_INTERRUPT);
        return ((reg >> 31) != 0);
    }
    
    
    private int getEnableInterruptResult(boolean isEnable) {
        int reg = this.getRegExe(REG_INTERRUPT);
        if(isEnable) {
            reg = reg | 0x80000000;
        } else {
            reg = reg & 0x7FFFFFFF;
        }
        return reg;
    }

    private boolean hasHWInterrupt() {
        if(!this.INTEnable()) return false;
        if(bus.isInterrupted) return true;
        return false;
    }

    private boolean canStartProcessInterrupt() {
        
        if(null == memAccessedInstruciton && 
           null == executedInstruction &&
           null == decodedInstruction &&
           null == fetchedInstruction)
            return true;
        else
            return false;
    }
    
    private boolean processHWI() {
        if(!canStartProcessInterrupt()) return false;
        
        this.setReg(REG_LRI, programCounter);
        programCounter = 4;
        //enableInterrupt(false);
        this.registers[this.REG_INTERRUPT] = getEnableInterruptResult(false);
        return true;
    }
    
    private boolean processSWI()
    {
        if(!canStartProcessInterrupt()) return false;
        this.registers[this.REG_INTERRUPT] = getEnableInterruptResult(false);
        setReg(REG_LRI, programCounter + 4);
        programCounter = 8;
        saveReg8_15();
        return true;
    }
    
    
    
    private boolean processRESUME()
    {
        if(!canStartProcessInterrupt()) return false;
        
        this.setReg(this.REG_INTERRUPT, getEnableInterruptResult(true));
        //regIntResult = getEnableInterruptResult(true);
        //setWrBkRegister(result, this.REG_INTERRUPT, regIntResult);
        restoreReg8_15();
        this.setReg(this.REG_RING, 3);
        programCounter = getReg(REG_LRI);
        return true;
    }

    private boolean fetch() {
        if(hasHWInterrupt()) {
            return processHWI();
            
        }else {
            if(null != fetchedInstruction) return false;
            if(!fetchStageCheck()) return false;
            if(fetchInstruction()) {
                int opCode = this.fetchedInstruction.getOpCode();
                if(opCode == OpCodeTable.SWI) {
                    if(processSWI()) {
                        fetchedInstruction = null;
                        return true;
                    }
                } else if(opCode == OpCodeTable.RESUME) {
                    if(processRESUME()) {
                        fetchedInstruction = null;
                        return true;
                    }
                } else {
                    return true;
                }
                
            }
        }
        return false;
    }
    
    private boolean fetchStageCheck() {
        if(null != fetchedInstruction) {
            if(fetchedInstruction.isBranch()) return false;
        }
        if(null != decodedInstruction) {
            if(decodedInstruction.isBranch()) return false;
        }
        return true;
    }
    
   
    
    
    private boolean decode(){
        if(null != decodedInstruction) return false;
        
        decodedInstruction = this.fetchedInstruction;
        this.fetchedInstruction = null;
        return true;
    }

    
    private boolean execute() {
        //only when memory access has finished can execute continue
        //it prevents data not ready from load or SWAP
        if(null != executedInstruction) return false;
        
        if(null != decodedInstruction) {
            executedInstruction = new ExecutedInstruction(decodedInstruction);
            execute(decodedInstruction, executedInstruction);
            decodedInstruction = null;
        }
        return true;
    }

    private void setWrBkRegister(ExecutedInstruction result, int targetReg, int data) 
    {
        result.requireWriteBack = true;
        result.regData = data;
        result.targetRegister = targetReg;
        //result.memoryAccess = memoryAccess;
    }

    private void execute(DecodedInstruction instr, ExecutedInstruction result) {
        //boolean pcAdd4 = true;
        int regIntResult;
        switch(instr.getOpCode()){
            case OpCodeTable.ADD:   //Ri = Rj + Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())+getRegExe(instr.getRegk()));
                break;
            case OpCodeTable.AND:   //Ri = Rj & Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) & getRegExe(instr.getRegk()));
                break;
            case OpCodeTable.DIV:   //Ri = Rj / Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) / getRegExe(instr.getRegk()));
                break;    
            case OpCodeTable.MUL:   //Ri = Rj * Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) * getRegExe(instr.getRegk()));                    
                break;

            case OpCodeTable.NAND:  //Ri = ~( Rj & Rk )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) & getRegExe(instr.getRegk())));
                break;
            case OpCodeTable.NXOR:  //Ri = ~( Rj ^ Rk )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) ^ getRegExe(instr.getRegk())));
                break;
            case OpCodeTable.NOR:   //Ri = ~( Rj | Rk )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) | getRegExe(instr.getRegk())));
                break;
            case OpCodeTable.OR:    //Ri = Rj | Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) | getRegExe(instr.getRegk()));
                break;
            case OpCodeTable.XOR:   //Ri = Rj ^ Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) ^ getRegExe(instr.getRegk()));
                break;
            case OpCodeTable.SUB:   //Ri = Rj - Rk
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj()) - getRegExe(instr.getRegk()));
                break;
            case OpCodeTable.SUBI:  //Ri = Rj - x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())-instr.getImmediate());
                break;
            case OpCodeTable.ADDI:  //Ri = Rj + x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())+instr.getImmediate());
                break;
            case OpCodeTable.ANDI:  //Ri = Rj & x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())&instr.getImmediate());
                break;
            case OpCodeTable.DIVI:  //Ri = Rj / x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())/instr.getImmediate());
                break;
            case OpCodeTable.MULI:  //Ri = Rj * x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())*instr.getImmediate());
                break;
            case OpCodeTable.NXORI: //Ri = ~( Rj ^ x )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) ^ instr.getImmediate()));
                break;
            case OpCodeTable.NANDI: //Ri = ~( Rj & x )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) & instr.getImmediate()));
                break;
            case OpCodeTable.XORI:  //Ri = Rj ^ x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())^instr.getImmediate());
                break;
            case OpCodeTable.ORI:   //Ri = Rj | x
                setWrBkRegister(result, instr.getRegi(), 
                    getRegExe(instr.getRegj())|instr.getImmediate());
                break;
            case OpCodeTable.NORI:  //Ri = ~( Rj | x )
                setWrBkRegister(result, instr.getRegi(), 
                    ~(getRegExe(instr.getRegj()) | instr.getImmediate()));
                break;
            case OpCodeTable.MOVI:  //Rj = immediate
                setWrBkRegister(result, instr.getRegj(), instr.getImmediate());
                break;



            case OpCodeTable.HALT:  //Stop execution & print registers
                result.isHalt = true;
                break;
            case OpCodeTable.LOAD:  //Ri=Mem[Rj+x]
                
                result.memoryAddress = getRegExe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = true;
                result.memoryReadThenWrite = false;
                result.memoryAccess = true;
                setWrBkRegister(result, instr.getRegi(), 0);
                break;
            case OpCodeTable.STORE: //Mem[Rj+x]=Rk
                result.memoryAccess = true;
                result.memoryAddress = getRegExe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = false;
                result.memoryReadThenWrite = false;
                result.memData = getRegExe(instr.getRegk());
                break;
            case OpCodeTable.SWAP:  //swap Mem[Rj+x] and Rk
                result.memoryAccess = true;
                result.memoryAddress = getRegExe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = false;
                result.memoryReadThenWrite = true;
                result.requireWriteBack = true;
                result.memData = getRegExe(instr.getRegk());
                setWrBkRegister(result, instr.getRegk(), 0);
                break;
            case OpCodeTable.DISABLE: //Disable interrupts
                //enableInterrupt(false);
                regIntResult = getEnableInterruptResult(false);
                setWrBkRegister(result, this.REG_INTERRUPT, regIntResult);
                break;               
            case OpCodeTable.ENABLE://Enable interrupts
                //enableInterrupt(true);
                regIntResult = getEnableInterruptResult(true);
                setWrBkRegister(result, this.REG_INTERRUPT, regIntResult);
                break;    
            case OpCodeTable.SWI:   //ENABLE=0; LRI = PC; PC = 8
                                    //r8--r15 should be saved away.
                                    //Argument passed in r0
                assert(false);
                //enableInterrupt(false);
                this.registers[this.REG_INTERRUPT] = getEnableInterruptResult(false);
                setReg(REG_LRI, programCounter + 4);
                programCounter = 8;
                saveReg8_15();
                //pcAdd4 = false;
                break;
            case OpCodeTable.RESUME: //PC = LRI; ENABLE=1;
                                     //r8--r15 should be restored.
                                     //Result passed in r0
                assert(false);
                //enableInterrupt(true);
                regIntResult = getEnableInterruptResult(true);
                setWrBkRegister(result, this.REG_INTERRUPT, regIntResult);
                //programCounter = getRegSafe(REG_LRI);
                //RING~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                restoreReg8_15();
                result.requireModifyPC = true;
                result.newPC = getRegExe(REG_LRI);
                //pcAdd4 = false;
                break;
            case OpCodeTable.NOP:   //
                break;
            case OpCodeTable.B:     //PC = Rj
                result.requireModifyPC = true;
                result.newPC = getRegExe(instr.getRegj());
                //pcAdd4=false;
                break;
            case OpCodeTable.BEQZ:  //IF Rj=0  THEN PC=PC+x
                if(getRegExe(instr.getRegj()) == 0) {
                    result.requireModifyPC = true;
                    result.newPC = instr.instructionAddress + instr.getImmediate();
                    //pc += instr.getImmediate();
                    //pcAdd4=false;
                }
                break;
            case OpCodeTable.BNEZ:  //IF Rj!=0 THEN PC=PC+x
                if(getRegExe(instr.getRegj()) != 0) {
                    result.requireModifyPC = true;
                    result.newPC = instr.instructionAddress + instr.getImmediate();
                    //pcAdd4=false;
                }
                break;
            case OpCodeTable.BGEZ:  //IF Rj>=0 THEN PC=PC+x
                if(getRegExe(instr.getRegj()) >= 0) {
                    result.requireModifyPC = true;
                    result.newPC = instr.instructionAddress + instr.getImmediate();
                    //pcAdd4=false;
                }
                break;
            case OpCodeTable.BLTZ:  //IF Rj<0  THEN PC=PC+x
                if(getRegExe(instr.getRegj()) < 0) {
                    result.requireModifyPC = true;
                    result.newPC = instr.instructionAddress + instr.getImmediate();
                    //pcAdd4=false;
                }
                break;
            case OpCodeTable.BR:    //PC = PC+x
                result.requireModifyPC = true;
                result.newPC = instr.instructionAddress + instr.getImmediate();
                //pc = pc + instr.getImmediate();
                //pcAdd4 = false;
                break;
            case OpCodeTable.BLR:   //LR = PC; PC = PC+x
                //setReg(REG_LR, pc + 4);
                //check-----------------------------------------------
                setWrBkRegister(result, REG_LR, instr.instructionAddress+4);
                result.requireModifyPC = true;
                result.newPC = instr.instructionAddress + instr.getImmediate();
                //pcAdd4 = false;
                break;

            case OpCodeTable.BL:    //LR = PC; PC = Rj
                //setReg(REG_LR, pc + 4);
                //check-----------------------------------------------
                setWrBkRegister(result, REG_LR, instr.instructionAddress+4);
                result.requireModifyPC = true;
                result.newPC = getRegExe(instr.getRegj());
                //pc = getRegSafe(instr.getRegj());
                //pcAdd4 = false;
                break;
            case OpCodeTable.RETURN: //PC = LR
                
                result.requireModifyPC = true;
                result.newPC = getRegExe(REG_LR);
                //pc = getRegSafe(REG_LR);
                //pcAdd4 = false;
                break;
        }
        //if(pcAdd4) {
        //    pc+=4;
        //}
    }

    private int temp[] = new int[8];
    private void saveReg8_15() {
        for(int i=0; i<8; ++i) {
            temp[i] = getRegExe(8+i);
        }
    }

    private void restoreReg8_15() {
        for(int i=0; i<8; ++i) {
            setReg(8+i, temp[i]);
        }
    }

    private boolean memoryAccess() {
        if(null != memAccessedInstruciton) return false;
        
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

    private boolean readMemory(ExecutedInstruction instr) 
    {
        mmu.readData(instr.memoryAddress);
        if(this.mmu.getDataReady()) {
            instr.regData = this.mmu.getData();
            return true;
        } else {
            return false;
        }
    }
    
    private boolean accessMemory() {
        
        if(executedInstruction.memoryReadThenWrite) {
            mmu.readData(executedInstruction.memoryAddress);
            if(readMemory(executedInstruction)) {
                //Convert to memory write, so in the next cycle 
                //it will do memory write
                executedInstruction.memoryReadThenWrite = false;
                executedInstruction.memoryRWbar = false;
            }
            return false;
            
        } else {
            if(executedInstruction.memoryRWbar) {
                return readMemory(executedInstruction);
            } else {
                mmu.writeData(executedInstruction.memoryAddress, 
                              executedInstruction.memData);  
                return true;
            }    
        }
        
    }

    private boolean writeBack() {
        
        if(null != memAccessedInstruciton) {
            if(memAccessedInstruciton.requireWriteBack) {

                setReg(memAccessedInstruciton.targetRegister, 
                        memAccessedInstruciton.regData);
            }
            
            if(memAccessedInstruciton.isHalt) {
                isHalt = true;
            }
            memAccessedInstruciton = null;
        }
        return true;
    }



    private boolean fetchInstruction(){
        this.mmu.readInstruction(this.programCounter);
        if(this.mmu.getInstrReady()) {
            //this.fetchedInstruction = this.mmu.getInstruction();
            fetchedInstruction = new DecodedInstruction(
                    this.mmu.getInstruction(), this.programCounter);
            //System.out.println(Integer.toBinaryString(this.fetchedInstruction));

            return true;
        }
        return false;
    }


    public boolean isHalt() {
        return this.isHalt;
    }

    private int getRegExe(int regNumber) {
        //executedInstruction should be ready so it can execute
        if(null != this.memAccessedInstruciton && 
                this.memAccessedInstruciton.requireWriteBack &&
                this.memAccessedInstruciton.targetRegister == regNumber) {
            return this.memAccessedInstruciton.regData;
        }
            
        return registers[regNumber];
    }
    
    public void setReg(int regNumber, int value) {
        registers[regNumber] = value;
    }
    
    //don't use, only for debugging
    public int getReg(int regNumber) {
        return registers[regNumber];
    }



    //only for debugging
    public int getRing() {
        return registers[REG_RING];
    }

    public void setRing(int ringLevel) {
            registers[REG_RING] = ringLevel;
    }

    public void setPC(int address) {
            this.programCounter = address;
    }

    public void executeOpcode(int instruction) {
        int op = getOp(instruction);
        int tempPC = programCounter;
        decodedInstruction = new DecodedInstruction(instruction, programCounter);
        executedInstruction = new ExecutedInstruction(decodedInstruction);
        execute(decodedInstruction, executedInstruction);
        writeBack();
        decodedInstruction = null;
        executedInstruction = null;
        programCounter = tempPC;
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