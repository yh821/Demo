local __bt__ = {
  file= "RootNode",
  data= {
    restart= 1
  },
  children= {
    {
      file= "SelectorNode",
      type= "composites/SelectorNode",
      data= {
        abort= "None"
      },
      children= {
        {
          file= "ParallelNode",
          type= "composites/ParallelNode",
          data= {
            abort= "None"
          },
          children= {
            {
              file= "LogNode",
              type= "actions/common/LogNode",
              data= {
                msg= 111
              },
            }
          }
        },
        {
          file= "ParallelNode",
          type= "composites/ParallelNode",
          data= {
            abort= "None"
          },
          children= {
            {
              file= "LogNode",
              type= "actions/common/LogNode",
              data= {
                msg= 222
              },
            }
          }
        }
      }
    }
  }
}
return __bt__