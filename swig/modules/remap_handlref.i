//%typemap(imtype) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "System.IntPtr"
//%typemap(csin) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "$csinput.Pointer"

%typemap(csbody) SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) %{

  private volatile System.IntPtr swigCPtr;
  
  public $csclassname(System.Runtime.InteropServices.HandleRef hRef, bool meanless) : this(hRef.Handle)
  {
    
  }
  
  public $csclassname(System.Runtime.InteropServices.HandleRef hRef) : this(hRef, true)
  {
    
  }
  
  public $csclassname(System.IntPtr ptr, bool meanless) : this(ptr)
  {
  
  }
  
  public $csclassname(System.IntPtr ptr)
  {
    swigCPtr = ptr;
  }
  
  public $csclassname()
  {
    swigCPtr = System.IntPtr.Zero;
  }
  
  public static System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj)
  {
    return new System.Runtime.InteropServices.HandleRef(obj, obj.swigCPtr);
  }

  public System.IntPtr Pointer => swigCPtr;
%}