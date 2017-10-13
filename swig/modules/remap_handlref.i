%typemap(imtype) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "System.IntPtr"
%typemap(csin) SWIGTYPE, SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) "$csinput.Pointer"

%typemap(csbody) SWIGTYPE *, SWIGTYPE &, SWIGTYPE [], SWIGTYPE (CLASS::*) %{

  private volatile System.IntPtr swigCPtr;

  protected $csclassname() 
  {
    swigCPtr = System.IntPtr.Zero;
  }

  internal System.IntPtr Pointer
  {
    get
    {
      return swigCPtr;
    }
  }
%}