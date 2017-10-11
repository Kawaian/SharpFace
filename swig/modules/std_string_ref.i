/* -----------------------------------------------------------------------------
 * std_string_ref.i
 *
 * Typemaps for std::string& and const std::string&
 * These are mapped to a C# String and are passed around by reference
 *
 * ----------------------------------------------------------------------------- */

%{
#include <string>
%}

namespace std {

%naturalvar string;

class string;

// string &

%typemap(ctype) std::string & "char**"
%typemap(imtype) std::string & "/*imtype*/ out string"
%typemap(cstype) std::string & "/*cstype*/ out string"

//C++
%typemap(in, canthrow=1) std::string &
%{  //typemap in
    std::string temp;
    $1 = &temp; 
 %}

//C++
%typemap(argout) std::string & 
%{ 
    //Typemap argout in c++ file.
    //This will convert c++ string to c# string
    *$input = SWIG_csharp_string_callback($1->c_str());
%}

%typemap(argout) const std::string & 
%{ 
    //argout typemap for const std::string&
%}

%typemap(csin) std::string & "out $csinput"

%typemap(throws, canthrow=1) string &
%{ SWIG_CSharpSetPendingException(SWIG_CSharpApplicationException, $1.c_str());
   return $null; %}

}