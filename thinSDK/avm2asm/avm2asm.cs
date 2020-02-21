using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinNeo.VM;

namespace ThinNeo.Compiler
{
    public class Avm2Asm
    {
        public static Op[] Trans(byte[] script)
        {
            ByteReader breader = new ByteReader(script);
            List<Op> arr = new List<Op>();
            while (breader.End == false)
            {
                Op o = new Op();
                o.addr = (UInt16)breader.addr;
                o.code = breader.ReadOP();
                try
                {
                    switch (o.code)
                    {
                        //Push
                        case OpCode.PUSHINT8:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)1;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHINT16:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)2;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHINT32:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)4;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHINT64:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)8;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHINT128:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)16;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHINT256:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = (int)32;
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHA:
                            {
                                o.paramType = ParamType.None;
                            }
                            break;
                        case OpCode.PUSHNULL:
                            {
                                o.paramType = ParamType.None;
                            }
                            break;
                        case OpCode.PUSHDATA1:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = breader.ReadByte();
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHDATA2:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = breader.ReadUInt16();
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHDATA4:
                            {
                                o.paramType = ParamType.ByteArray;
                                var count = breader.ReadInt32();
                                o.paramData = breader.ReadBytes(count);
                            }
                            break;
                        case OpCode.PUSHM1:
                        case OpCode.PUSH0:
                        case OpCode.PUSH1:
                        case OpCode.PUSH2:
                        case OpCode.PUSH3:
                        case OpCode.PUSH4:
                        case OpCode.PUSH5:
                        case OpCode.PUSH6:
                        case OpCode.PUSH7:
                        case OpCode.PUSH8:
                        case OpCode.PUSH9:
                        case OpCode.PUSH10:
                        case OpCode.PUSH11:
                        case OpCode.PUSH12:
                        case OpCode.PUSH13:
                        case OpCode.PUSH14:
                        case OpCode.PUSH15:
                        case OpCode.PUSH16:
                            {
                                o.paramType = ParamType.None;
                            }
                            break;
                        // Control
                        case OpCode.NOP:
                            {
                                o.paramType = ParamType.None;
                            } 
                            break;
                        case OpCode.JMP:
                        case OpCode.JMPIF:
                        case OpCode.JMPIFNOT:
                        case OpCode.JMPEQ:
                        case OpCode.JMPNE:
                        case OpCode.JMPGT:
                        case OpCode.JMPGE:
                        case OpCode.JMPLT:
                        case OpCode.JMPLE:
                            {
                                o.paramType = ParamType.Addr;
                                o.paramData = breader.ReadBytes(1);
                            }
                            break;
                        case OpCode.JMP_L:
                        case OpCode.JMPIF_L:
                        case OpCode.JMPIFNOT_L:
                        case OpCode.JMPEQ_L:
                        case OpCode.JMPNE_L:
                        case OpCode.JMPGT_L:
                        case OpCode.JMPGE_L:
                        case OpCode.JMPLT_L:
                        case OpCode.JMPLE_L:
                            {
                                o.paramType = ParamType.Addr;
                                o.paramData = breader.ReadBytes(4);
                            }
                            break;
                        case OpCode.CALL:
                            {
                                o.paramType = ParamType.Addr;
                                o.paramData = breader.ReadBytes(1);
                                break;
                            }
                        case OpCode.CALL_L:
                            {
                                o.paramType = ParamType.Addr;
                                o.paramData = breader.ReadBytes(4);
                                break;
                            }
                        case OpCode.CALLA:
                            {
                                o.paramType = ParamType.Addr;
                                o.paramData = breader.ReadBytes(4);
                                break;
                            }
                        case OpCode.RET:
                        case OpCode.THROW:
                        case OpCode.THROWIF:
                        case OpCode.THROWIFNOT:
                            {
                                o.paramType = ParamType.None;
                            }
                            break;
                        case OpCode.SYSCALL:
                            {
                                o.paramType = ParamType.String;
                                o.paramData = breader.ReadVarBytes(252);
                            }
                            break;
                        // Stack ops
                        case OpCode.DEPTH:
                        case OpCode.DROP:
                        case OpCode.NIP:
                        case OpCode.XDROP:
                        case OpCode.CLEAR:
                        case OpCode.DUP:
                        case OpCode.OVER:
                        case OpCode.PICK:
                        case OpCode.TUCK:
                        case OpCode.SWAP:
                        case OpCode.ROT:
                        case OpCode.ROLL:
                        case OpCode.REVERSE3:
                        case OpCode.REVERSE4:
                        case OpCode.REVERSEN:
                        //Slot
                        case OpCode.INITSSLOT:
                        case OpCode.INITSLOT:
                        case OpCode.LDSFLD0:
                        case OpCode.LDSFLD1:
                        case OpCode.LDSFLD2:
                        case OpCode.LDSFLD3:
                        case OpCode.LDSFLD4:
                        case OpCode.LDSFLD5:
                        case OpCode.LDSFLD6:
                        case OpCode.LDSFLD:
                        case OpCode.STSFLD0:
                        case OpCode.STSFLD1:
                        case OpCode.STSFLD2:
                        case OpCode.STSFLD3:
                        case OpCode.STSFLD4:
                        case OpCode.STSFLD5:
                        case OpCode.STSFLD6:
                        case OpCode.STSFLD:
                        case OpCode.LDLOC0:
                        case OpCode.LDLOC1:
                        case OpCode.LDLOC2:
                        case OpCode.LDLOC3:
                        case OpCode.LDLOC4:
                        case OpCode.LDLOC5:
                        case OpCode.LDLOC6:
                        case OpCode.LDLOC:
                        case OpCode.STLOC0:
                        case OpCode.STLOC1:
                        case OpCode.STLOC2:
                        case OpCode.STLOC3:
                        case OpCode.STLOC4:
                        case OpCode.STLOC5:
                        case OpCode.STLOC6:
                        case OpCode.STLOC:
                        case OpCode.LDARG0:
                        case OpCode.LDARG1:
                        case OpCode.LDARG2:
                        case OpCode.LDARG3:
                        case OpCode.LDARG4:
                        case OpCode.LDARG5:
                        case OpCode.LDARG6:
                        case OpCode.LDARG:
                        case OpCode.STARG0:
                        case OpCode.STARG1:
                        case OpCode.STARG2:
                        case OpCode.STARG3:
                        case OpCode.STARG4:
                        case OpCode.STARG5:
                        case OpCode.STARG6:
                        case OpCode.STARG:
                        // Splice
                        case OpCode.NEWBUFFER:
                        case OpCode.MEMCPY:
                        case OpCode.CAT:
                        case OpCode.SUBSTR:
                        case OpCode.LEFT:
                        case OpCode.RIGHT:
                        // Bitwise logic
                        case OpCode.INVERT:
                        case OpCode.AND:
                        case OpCode.OR:
                        case OpCode.XOR:
                        case OpCode.EQUAL:
                        case OpCode.NOTEQUAL:
                        // Numeric
                        case OpCode.SIGN:
                        case OpCode.ABS:
                        case OpCode.NEGATE:
                        case OpCode.INC:
                        case OpCode.DEC:
                        case OpCode.ADD:
                        case OpCode.SUB:
                        case OpCode.MUL:
                        case OpCode.DIV:
                        case OpCode.MOD:
                        case OpCode.SHL:
                        case OpCode.SHR:
                        case OpCode.NOT:
                        case OpCode.BOOLAND:
                        case OpCode.BOOLOR:
                        case OpCode.NZ:
                        case OpCode.NUMEQUAL:
                        case OpCode.NUMNOTEQUAL:
                        case OpCode.LT:
                        case OpCode.LE:
                        case OpCode.GT:
                        case OpCode.GE:
                        case OpCode.MIN:
                        case OpCode.MAX:
                        case OpCode.WITHIN:
                        // Compound-type
                        case OpCode.PACK:
                        case OpCode.UNPACK:
                        case OpCode.NEWARRAY0:
                        case OpCode.NEWARRAY:
                        case OpCode.NEWARRAY_T:
                        case OpCode.NEWSTRUCT0:
                        case OpCode.NEWSTRUCT:
                        case OpCode.NEWMAP:
                        case OpCode.SIZE:
                        case OpCode.HASKEY:
                        case OpCode.KEYS:
                        case OpCode.VALUES:
                        case OpCode.PICKITEM:
                        case OpCode.APPEND:
                        case OpCode.SETITEM:
                        case OpCode.REVERSEITEMS:
                        case OpCode.REMOVE:
                        case OpCode.CLEARITEMS:

                        //Types
                        case OpCode.ISNULL:
                        case OpCode.ISTYPE:
                        case OpCode.CONVERT:
                            o.paramType = ParamType.None;
                            break;
                        default: 
                            throw new Exception("you fogot a type:" + o.code);
                    }
                }
                catch
                {
                    o.error = true;
                }
                arr.Add(o);
                if (o.error)
                    break;
            }
            return arr.ToArray();
        }
    }
}
