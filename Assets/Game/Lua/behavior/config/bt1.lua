local __bt__ = {
  file= "RootNode",
  type= "",
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
              file= "WaitNode",
              type= "actions/common/WaitNode",
              data= {
                min_time= 1,
                max_time= 1
              },
            },
            {
              file= "IsActiveNode",
              type= "conditions/IsActiveNode",
              data= {
                path= "Sprite"
              },
            },
            {
              file= "SequenceNode",
              type= "composites/SequenceNode",
              data= {
                abort= "Lower"
              },
              children= {
                {
                  file= "IsActiveNode",
                  type= "conditions/IsActiveNode",
                  data= {
                    path= "BottomHint"
                  },
                },
                {
                  file= "SequenceNode",
                  type= "composites/SequenceNode",
                  data= {
                    abort= "Lower"
                  },
                  children= {
                    {
                      file= "IsActiveNode",
                      type= "conditions/IsActiveNode",
                      data= {
                        path= "Open"
                      },
                    },
                    {
                      file= "SequenceNode",
                      type= "composites/SequenceNode",
                      data= {
                        abort= "Lower"
                      },
                      children= {
                        {
                          file= "IsActiveNode",
                          type= "conditions/IsActiveNode",
                          data= {
                            path= "Label"
                          },
                        },
                        {
                          file= "WaitNode",
                          type= "actions/common/WaitNode",
                          data= {
                            min_time= 1,
                            max_time= 1
                          },
                        }
                      }
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
                max_time= 1
              },
            }
          }
        },
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
                abort= "None"
              },
              children= {
                {
                  file= "WaitNode",
                  type= "actions/common/WaitNode",
                  data= {
                    min_time= 1,
                    max_time= 1
                  },
                },
                {
                  file= "WaitNode",
                  type= "actions/common/WaitNode",
                  data= {
                    min_time= 1,
                    max_time= 1
                  },
                }
              }
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
              file= "IsActiveNode",
              type= "conditions/IsActiveNode",
              data= {
                path= "ScrollView"
              },
            },
            {
              file= "WaitNode",
              type= "actions/common/WaitNode",
              data= {
                min_time= 1,
                max_time= 1
              },
            }
          }
        }
      }
    }
  }
}
return __bt__