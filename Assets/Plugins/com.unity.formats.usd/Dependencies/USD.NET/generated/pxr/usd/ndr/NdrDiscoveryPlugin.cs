//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 3.0.12
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace pxr
{
    public class NdrDiscoveryPlugin : global::System.IDisposable
    {
        private global::System.Runtime.InteropServices.HandleRef swigCPtr;
        protected bool swigCMemOwn;

        internal NdrDiscoveryPlugin(global::System.IntPtr cPtr, bool cMemoryOwn)
        {
            swigCMemOwn = cMemoryOwn;
            swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
        }

        internal static global::System.Runtime.InteropServices.HandleRef getCPtr(NdrDiscoveryPlugin obj)
        {
            return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
        }

        ~NdrDiscoveryPlugin()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            lock (this) {
                if (swigCPtr.Handle != global::System.IntPtr.Zero)
                {
                    if (swigCMemOwn)
                    {
                        swigCMemOwn = false;
                        UsdCsPINVOKE.delete_NdrDiscoveryPlugin(swigCPtr);
                    }
                    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
                }
                global::System.GC.SuppressFinalize(this);
            }
        }

        public virtual NdrNodeDiscoveryResultVector DiscoverNodes(NdrDiscoveryPluginContext arg0)
        {
            NdrNodeDiscoveryResultVector ret = new NdrNodeDiscoveryResultVector(UsdCsPINVOKE.NdrDiscoveryPlugin_DiscoverNodes(swigCPtr, NdrDiscoveryPluginContext.getCPtr(arg0)), true);
            if (UsdCsPINVOKE.SWIGPendingException.Pending) throw UsdCsPINVOKE.SWIGPendingException.Retrieve();
            return ret;
        }

        public virtual StdStringVector GetSearchURIs()
        {
            StdStringVector ret = new StdStringVector(UsdCsPINVOKE.NdrDiscoveryPlugin_GetSearchURIs(swigCPtr), false);
            return ret;
        }
    }
}