local __bt__ = {
  file= "RootNode",
  type= "",
  sharedData= {
    tt1= 1
  },
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
          file= "SequenceNode",
          type= "composites/SequenceNode",
          data= {
            abort= "Lower"
          },
          children= {
            {
              file= "IsInViewNode",
              type= "conditions/IsInViewNode",
              data= {
                ViewRange= 3
              },
            },
            {
              file= "MoveToPositionNode",
              type= "actions/MoveToPositionNode",
              data= {
                pos= "TargetPos"
              },
            },
            {
              file= "AttackNode",
              type= "actions/AttackNode",
              data= {
                pos= "TargetPos"
              },
            }
          }
        },
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
              file= "WaitNode",
              type= "actions/common/WaitNode",
              data= {
                min_time= 1,
                max_time= 3
              },
            }
          }
        },
        {
          file= "SequenceNode",
          type= "composites/SequenceNode",
          data= {
            abort= "None"
          },
          children= {
            {
              file= "RandomPositionNode",
              type= "actions/RandomPositionNode",
              data= {
                center= "0,0,0",
                range= 5
              },
            },
            {
              file= "MoveToPositionNode",
              type= "actions/MoveToPositionNode",
              data= {
                pos= "RandomPos"
              },
            }
          }
        }
      }
    }
  }
}
return __bt__