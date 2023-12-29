local __bt__ = {
  file= "RootNode",
  type= "",
  data= {
    restart= 1
  },
  children= {
    {
      file= "ParallelNode",
      type= "composites/ParallelNode",
      children= {
        {
          file= "LogNode",
          type= "actions/common/LogNode",
        },
        {
          file= "LogNode",
          type= "actions/common/LogNode",
        },
        {
          file= "LogNode",
          type= "actions/common/LogNode",
        }
      }
    }
  }
}
return __bt__