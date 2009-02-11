

//package asm;


public class AsmMain {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		
		
		//System.out.println(new File(System.getProperty("user.dir")));
		String inFile = null;
		String outFile = null;
		if(args.length != 2) {
			System.out.println(args.length);
			System.out.println("Please enter input, output filenames.");
			return;
			//Asm.assemble(inFile, outFile);
		} else {
			inFile = args[0];
			outFile = args[1];
			/*
			if(inFile.contains(".")){
				int idx = inFile.indexOf(".");
				if(inFile.substring(idx) != "out"){
					outFile = inFile.substring(0, idx-1);
				}
			}
			outFile += ".out";
			*/
		}
		try {
			Asm.assemble(inFile, outFile);
		} catch(Exception e) {
			System.out.println(e.toString());
		}
		
/*
		
		*/
	}

}
	
