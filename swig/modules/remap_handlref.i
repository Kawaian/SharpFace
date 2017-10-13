%typemap(imtype) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "System.IntPtr"
%typemap(csin) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "$csinput.Pointer"

%typemap(csbody) SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) %{

  private volatile System.IntPtr swigCPtr;
  
  public $csclassname(System.Runtime.InteropServices.HandleRef hRef)
  {
    swigCPtr = hRef.Handle;
  }
  
  public $csclassname(System.IntPtr ptr)
  {
    swigCPtr = ptr;
  }
  
  public $csclassname()
  {
    swigCPtr = System.IntPtr.Zero;
  }

  public System.IntPtr Pointer => swigCPtr;
%}