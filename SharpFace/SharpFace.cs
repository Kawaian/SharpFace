using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SharpFace
{
    public class Test
    {
        public void Invoke()
        {
            LandmarkDetector.CalculateBox(null, 0, 0, 0, 0);
        }
    }
}

namespace System.Runtime.InteropServices
{
    /// <summary>
    /// NAH I DONT KNOW WHAT IS THIS.
    /// </summary>
    [ComVisible(true)]
    public struct HandleRef
    {
        internal object m_wrapper;

        internal IntPtr m_handle;

        /// <summary>리소스에 핸들을 보유 하는 개체를 가져옵니다.</summary>
        /// <returns>리소스에 핸들을 보유 하는 개체입니다.</returns>
        public object Wrapper
        {
            get
            {
                return this.m_wrapper;
            }
        }

        /// <summary>리소스에 대 한 핸들을 가져옵니다.</summary>
        /// <returns>리소스에 대 한 핸들입니다.</returns>
        public IntPtr Handle
        {
            get
            {
                return this.m_handle;
            }
        }

        /// <summary>새 인스턴스를 초기화는 <see cref="T:System.Runtime.InteropServices.HandleRef" /> 래핑할 개체 및 비관리 코드에서 사용 하는 리소스에 대 한 핸들을 사용 하 여 클래스입니다.</summary>
        /// <param name="wrapper">플랫폼 호출 될 때까지 종료 해서는 안 하는 관리 되는 개체를 반환 합니다.</param>
        /// <param name="handle">
        ///   <see cref="T:System.IntPtr" /> 리소스에 대 한 핸들을 나타내는입니다.</param>
        public HandleRef(object wrapper, IntPtr handle)
        {
            this.m_wrapper = wrapper;
            this.m_handle = handle;
        }

        /// <summary>지정 된 리소스에 핸들을 반환 합니다. <see cref="T:System.Runtime.InteropServices.HandleRef" /> 개체입니다.</summary>
        /// <param name="value">핸들을 필요로 하는 개체입니다.</param>
        /// <returns>지정 된 리소스에 대 한 핸들 <see cref="T:System.Runtime.InteropServices.HandleRef" /> 개체입니다.</returns>
        public static explicit operator IntPtr(HandleRef value)
        {
            return value.m_handle;
        }

        /// <summary>내부 정수 표현을 반환는 <see cref="T:System.Runtime.InteropServices.HandleRef" /> 개체입니다.</summary>
        /// <param name="value">A <see cref="T:System.Runtime.InteropServices.HandleRef" /> 에서 내부 정수 표현을 검색할 개체입니다.</param>
        /// <returns>
        ///   <see cref="T:System.IntPtr" /> 을 나타내는 개체를 <see cref="T:System.Runtime.InteropServices.HandleRef" /> 개체입니다.</returns>
        public static IntPtr ToIntPtr(HandleRef value)
        {
            return value.m_handle;
        }
    }
}
