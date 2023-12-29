local __bt__ = {
  file= "RootNode",
  data= {
    restart= 1
  },
  children= {
    {
      file= "SequenceNode",
      type= "composites/SequenceNode",
      data= {
        abort= "None"
      },
      children= {
        {
          file= "SequenceNode",
          type= "composites/SequenceNode",
          data= {
            abort= "None"
          },
          children= {
            {
              file= "WeightNode",
              type= "actions/common/WeightNode",
              data= {
                weight= 500
              },
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
                    msg= 111
                  },
                },
                {
                  file= "WaitNode",
                  type= "actions/common/WaitNode",
                  data= {
                    min_time= 1,
                    max_time= 3
                  },
                },
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
        },
        {
          file= "WaitNode",
          type= "actions/common/WaitNode",
          data= {
            min_time= 1,
            max_time= 3
          },
        },
        {
          file= "WaitNode",
          type= "actions/common/WaitNode",
          data= {
            min_time= 1,
            max_time= 3
          },
        },
        {
          file= "LogNode",
          type= "actions/common/LogNode",
          data= {
            msg= ""
          },
        }
      }
    }
  }
}
return __bt__