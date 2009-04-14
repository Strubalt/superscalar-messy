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
public class Cache {
    
    private boolean enable;
    
    private static class CacheEntry {
        
        int Address, Data;
      
        CacheEntry(int addr, int data) {
          
            Address = addr;
            Data = data;
        }
    }
    
    
    private ArrayList<ArrayList<CacheEntry>> sets;
    private int numWay;
    
    public int getNumSet() {
        return sets.size();
    }
    Cache(int nSet, int nWay) {
        if(nSet == 0 | nWay == 0) enable = false;
        else enable = true;
        
        if(enable) {
            numWay = nWay;
            sets = new ArrayList<ArrayList<CacheEntry>>();
            for(int i=0; i<nSet; ++i) {
                sets.add(new ArrayList<CacheEntry>());
            }
        } else {
            sets = new ArrayList<ArrayList<CacheEntry>>();
            sets.add(new ArrayList<CacheEntry>());
        }
        
    }
    
    public boolean exist(int addr) {
        int setIdx = getSetIndex(addr);
        for(CacheEntry Entry: sets.get(setIdx)) {
            if(Entry.Address == addr) return true;
        }
        return false;
    }
    
    private int getSetIndex(int addr) {
        return (addr / 4)  % getNumSet();
    }
    
    public int read(int addr) {
        int setIdx = getSetIndex(addr);
        for(CacheEntry Entry: sets.get(setIdx)) {
            if(Entry.Address == addr) return Entry.Data;
        }
        assert(true);
        return 0x80000000;
        //return false;
    }
    
    public void write(int addr, int data) {
        if(enable) {
            int setIdx = getSetIndex(addr);
            int EntryIndex=-1;
            CacheEntry entry = null;
            ArrayList<CacheEntry> Entrys = sets.get(setIdx);
            for(int i=0; i<Entrys.size(); ++i) {
                if(Entrys.get(i).Address == addr) {
                    Entrys.get(i).Data = data;
                    EntryIndex = i;
                    entry = Entrys.get(i);
                }
            }
            if(EntryIndex != -1) {
                Entrys.remove(EntryIndex);
            }
            if(entry == null) {
                entry = new CacheEntry(addr, data);
            }
            if(Entrys.size() == numWay) {
                Entrys.remove(0);
            }
            Entrys.add(entry);
        }
    }
}
