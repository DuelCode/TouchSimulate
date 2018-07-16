# TouchSimulate
Simulate Touch Events

Base On：
  
   [DllImport("User32.dll")]
   public static extern bool InitializeTouchInjection(uint maxCount = 256, TouchFeedback feedbackMode = TouchFeedback.DEFAULT);

   [DllImport("User32.dll")]
   public static extern bool InjectTouchInput(int count, [MarshalAs(UnmanagedType.LPArray), In] PointerTouchInfo[] contacts);
   
My Environment：
  Win8.1 x64 Vs2017.
  

  
