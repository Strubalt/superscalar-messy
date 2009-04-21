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
    private boolean hasException;
    private int exceptionAddress;
    private INTType exceptionType;
    private MMU mmu = new MMU();
    private DecodedInstruction fetchedInstruction;
    private DecodedInstruction decodedInstruction;
    private ExecutedInstruction executedInstruction;
    private ExecutedInstruction memAccessedInstruciton;
    private int cycle;
    //private boolean INTEnable;
    private boolean isPipeline;



    public Processor(boolean isPipeline) {
            super(0);
            this.isPipeline = isPipeline;
            programCounter = 0;
            cycle = 0;
            stage = STG_FETCH;
            registers = new int[64];
            
            registers[REG_LIMIT] = 0x7FFFFFFF;
            registers[REG_RING] = 0;
            registers[REG_TIMER] = -1;  //disable Timer Interrupt
    }

    private void advanceCycle() {
        cycle += 1;
        int count = getReg(REG_TIMER);
        if(count > 0) this.setReg(REG_TIMER, count-1);
    }
    
    void advanceTimeBasic() {
        //cycle+=1;
        
        switch(stage) {
            case STG_FETCH:
                
                if(fetch()) {
                    advanceCycle();
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
                    if(executedInstruction.requireModifyPC)
                        programCounter = executedInstruction.newPC;
                    else
                        programCounter += 4;
                    
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
        this.mmu.advanceTime();
    }
    
    void advanceTimePipeline() {
        advanceCycle();
       
        writeBack();
        memoryAccess();
        execute();
        decode();
        boolean next=false;
        if(null != executedInstruction && executedInstruction.requireModifyPC) {
            
        } else {
            next = fetch();
        }
             //check if there is stall
        if(null != executedInstruction && executedInstruction.requireModifyPC) {
            programCounter = executedInstruction.newPC;
            this.fetchedInstruction = null;
            this.decodedInstruction = null;
        } else if(next) {
            programCounter += 4;
        }
        
        this.mmu.advanceTime();
    }
    
    @Override
    void advanceTime() {
        //advanceTimePipeline();
        if(this.isPipeline) this.advanceTimePipeline();
        else this.advanceTimeBasic();
    }

    private boolean processSWIandRESUME() {
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
        }
        return false;
    }
    
    private boolean fetch() {
        if(hasInterrupt()) {
            processInterrupt();
            return false;
        } else {
            if(null != fetchedInstruction) { 
                this.mmu.cancelInstructionRead();
                return false;
            }
           
            if(fetchInstruction()) {
                int opCode = this.fetchedInstruction.getOpCode();
                if(opCode == OpCodeTable.SWI | opCode == OpCodeTable.RESUME) {
                    processSWIandRESUME();
                    fetchedInstruction = null;
                } else {
                    return true;
                }

            }

            return false;            
        }
        

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
    
    int translateToPhysicalAddress(int virtualAddr) {
        if(0 != getRegExe(REG_RING)) {
            return getRegExe(REG_BASE) + virtualAddr;
        } else {
            return virtualAddr;
        }
    }
    
    private boolean fetchInstruction(){
        int physicalInstrAddr=translateToPhysicalAddress(programCounter);
        this.mmu.readInstruction(physicalInstrAddr);
        if(this.mmu.getInstrReady()) {
            //this.fetchedInstruction = this.mmu.getInstruction();
            fetchedInstruction = new DecodedInstruction(
                    this.mmu.getInstruction(), this.programCounter);
            //System.out.println(Integer.toBinaryString(this.fetchedInstruction));

            return true;
        }
        return false;
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
            if(decodedInstruction.numCycleToFinish > 1) {
                decodedInstruction.numCycleToFinish -= 1;
                return false;
            }
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
        int den;
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
                den = getRegExe(instr.getRegk());
                if(den != 0) {
                    setWrBkRegister(result, instr.getRegi(), 
                        getRegExe(instr.getRegj()) / getRegExe(instr.getRegk()));                    
                } else {
                    result.hasException = true;
                    result.exceptionType = INTType.DividByZero;
                }

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
                den = instr.getImmediate();
                if(den != 0) {
                    setWrBkRegister(result, instr.getRegi(), 
                        getRegExe(instr.getRegj())/instr.getImmediate());                 
                } else {
                    result.hasException = true;
                    result.exceptionType = INTType.DividByZero;
                }

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
                //System.out.println(result.memData);
                break;
            case OpCodeTable.SWAP:  //swap Mem[Rj+x] and Rk
                result.memoryAccess = true;
                result.memoryAddress = getRegExe(instr.getRegj())+instr.getImmediate();
                result.memoryRWbar = false;
                result.memoryReadThenWrite = true;
                result.memData = getRegExe(instr.getRegk());
                setWrBkRegister(result, instr.getRegk(), 0);
                break;
            case OpCodeTable.DISABLE: //Disable interrupts
                //enableInterrupt(false);
                regIntResult = getEnableInterruptValue(getRegExe(REG_INTERRUPT), false);
                setWrBkRegister(result, REG_INTERRUPT, regIntResult);
                break;               
            case OpCodeTable.ENABLE://Enable interrupts
                //enableInterrupt(true);
                regIntResult = getEnableInterruptValue(getRegExe(REG_INTERRUPT), true);
                setWrBkRegister(result, REG_INTERRUPT, regIntResult);
                break;    
            case OpCodeTable.SWI:   //ENABLE=0; LRI = PC; PC = 8
                                    //r8--r15 should be saved away.
                                    //Argument passed in r0
                assert(true);      //should process eariler
                
                break;
            case OpCodeTable.RESUME: //PC = LRI; ENABLE=1;
                                     //r8--r15 should be restored.
                                     //Result passed in r0
                assert(true);       //should process eariler
                
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
            default:
                assert(true);
        }
        //if(pcAdd4) {
        //    pc+=4;
        //}
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
    
    private boolean accessMemory() {
        
        if(executedInstruction.memoryReadThenWrite) {
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
                if(!mmu.canWriteData()) return false;
                int physicalAddress = translateToPhysicalAddress(executedInstruction.memoryAddress);
                       
                if((this.getRegExe(REG_RING)==0) || 
                    (physicalAddress <= getRegExe(REG_LIMIT)))  {
                    mmu.writeData(physicalAddress, executedInstruction.memData);  
                } else {
                    executedInstruction.hasException = true;
                    executedInstruction.exceptionType = INTType.PageFault;
                }
               
                return true;
            }    
        }
        
    }

    private boolean readMemory(ExecutedInstruction instr) 
    {
        int physicalAddress =translateToPhysicalAddress(instr.memoryAddress);;
        mmu.readData(physicalAddress);
        if(this.mmu.getDataReady()) {
            instr.regData = this.mmu.getData();
            return true;
        } else {
            return false;
        }
    }
    
    private boolean writeBack() {
        
        if(null != memAccessedInstruciton) {
            if(this.memAccessedInstruciton.hasException) {
                //don't have to worry about memory write
                //because writeBack will operate before memory write
                hasException = true;
                exceptionAddress = this.memAccessedInstruciton.instrAddress();
                exceptionType = this.memAccessedInstruciton.exceptionType;
                this.memAccessedInstruciton = null;
                this.executedInstruction = null;
                this.decodedInstruction = null;
                this.fetchedInstruction = null;
                //this.programCounter = exceptionAddress;
                return true;
            }
            if(memAccessedInstruciton.requireWriteBack) {
                int target = memAccessedInstruciton.targetRegister;
                boolean hasException=false;
                if(this.getReg(REG_RING) == 0) {
                    registers[target] = memAccessedInstruciton.regData;
                } else if(this.getReg(REG_RING) != 3) {
                    if(target <57 || target == REG_INTERRUPT) {
                        registers[target] = memAccessedInstruciton.regData;
                    } else {
                        hasException = true;
                    }
                } else {
                    if(target <57) {
                        registers[target] = memAccessedInstruciton.regData;
                    } else {
                        hasException = true;
                    }
                }     
                if(hasException) {
                    hasException = true;
                    exceptionAddress = this.memAccessedInstruciton.instrAddress();
                    exceptionType = INTType.AccessPrivilegeReg;
                }
            }
            
            if(memAccessedInstruciton.isHalt) {
                isHalt = true;
            }
            memAccessedInstruciton = null;
        }
        return true;
    }

    private boolean INTEnable(int regValue) {
        //int reg = this.getRegExe(REG_INTERRUPT);
        return ((regValue >> 31) != 0);
    }
    
    private int getEnableInterruptValue(int regIntValue, boolean isEnable) {
        if(isEnable) {
            return regIntValue | 0x80000000;
        } else {
            return regIntValue & 0x7FFFFFFF;
        }
    }
    
    private void directEnableInterrupt(boolean isEnable) {
        int reg = this.getReg(REG_INTERRUPT);
        reg = getEnableInterruptValue(reg, isEnable);
        setReg(REG_INTERRUPT, reg);
    }
    
    private void directSetINTType(INTType type) {
        int reg = this.getReg(REG_INTERRUPT);
        reg = type.setINT(reg);
        setReg(REG_INTERRUPT, reg);
    }

    private boolean hasInterrupt() {
        if(!this.INTEnable(this.getReg(REG_INTERRUPT))) return false;
        if(bus.isInterrupted) return true;
        if(this.hasException) return true;
        if(this.getReg(REG_TIMER) == 0) return true;
        return false;
    }

    private boolean canStartProcessInterrupt(boolean isSWInstr) {
        
        if(null == memAccessedInstruciton && 
           null == executedInstruction &&
           null == decodedInstruction) {
            if(isSWInstr) {
                return true;
            } 
            return (null == fetchedInstruction);
        } else {
            return false;
        }
    }
    

    
    private static enum ISR {
        HW(4), SW(8);
        
        public int Address;
        ISR(int addr) {
            Address = addr;
        }
    }
    
    private void processInterrupt() {
        if(!canStartProcessInterrupt(false)) return;
        //saveReg8_15();
        directEnableInterrupt(false);
        this.setReg(REG_RING, 0);
        if(bus.isInterrupted) {                     
            this.setReg(REG_LRI, programCounter);
            programCounter = ISR.HW.Address;
            this.directSetINTType(INTType.Interconnect);
            //enableInterrupt(false);
            //this.setReg(REG_INTERRUPT, getEnableInterruptResult(false));
            
        } else if(this.getReg(REG_TIMER) == 0) {    
            this.setReg(REG_LRI, programCounter);
            programCounter = ISR.HW.Address;
            this.setReg(REG_TIMER, -1);
            this.directSetINTType(INTType.Timer);
        }  else if(this.hasException) {         
            programCounter = this.exceptionAddress;
            this.setReg(REG_LRI, programCounter);
            programCounter = ISR.SW.Address;
            this.directSetINTType(exceptionType);
        } else {
            assert(true);
        }
        
        
    }
    
    //ENABLE=0; LRI = PC; PC = 8
    //r8--r15 should be saved away.
    //Argument passed in r0
    private boolean processSWI() {
        if(!canStartProcessInterrupt(true)) return false;
        //this.setRegRingChecked(REG_INTERRUPT, getEnableInterruptResult(false));
        directEnableInterrupt(false);
        this.setReg(REG_RING, 0);
        setReg(REG_LRI, programCounter+4);
        programCounter = ISR.SW.Address;
        //saveReg8_15();
        return true;

    }

    //PC = LRI; ENABLE=1;
    //r8--r15 should be restored.
    //Result passed in r0
    private boolean processRESUME() {
        if(!canStartProcessInterrupt(true)) return false;
        
        //this.setRegRingChecked(REG_INTERRUPT, getEnableInterruptResult(true));
        directEnableInterrupt(true);
        bus.isInterrupted = false;
        //restoreReg8_15();
        //this.setReg(REG_RING, 3);
        programCounter = getReg(REG_LRI);
        return true;
    }

    /*
    private int temp[] = new int[8];
    private void saveReg8_15() {
        for(int i=0; i<8; ++i) {
            temp[i] = getRegExe(8+i);
        }
    }

    private void restoreReg8_15() {
        for(int i=0; i<8; ++i) {
            setRegRingChecked(8+i, temp[i]);
        }
    }
    */
    
    void attachInterconnect(Interconnect connection) {
            // TODO Auto-generated method stub
            bus = connection;
            mmu.attachInterconnect(connection);
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
        return getReg(REG_RING);
    }

    public void setRing(int ringLevel) {            
        setReg(REG_RING, ringLevel);
    }

    public void setPC(int address) {
        this.programCounter = address;
    }

    public void executeOpcode(int instruction) {
        int op = getOp(instruction);
        int tempPC = programCounter;
        
        setReg(REG_RING,0);
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