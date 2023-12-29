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
        abort= "None"
      },
      children= {
        {
          file= "RandomPositionNode",
          type= "actions/RandomPositionNode",
          data= {
            center= "0,0,0",
            range= 2
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
return __bt__