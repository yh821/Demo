local __bt__ = {
  file= "RootNode",
  type= "",
  data= {
    restart= 1
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
    }
  }
}
return __bt__