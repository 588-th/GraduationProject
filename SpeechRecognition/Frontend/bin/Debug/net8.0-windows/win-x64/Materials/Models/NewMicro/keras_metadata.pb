
ÿ+root"_tf_keras_layer*ß+{
  "name": "sequential",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "class_name": "Sequential",
  "config": {
    "name": "sequential",
    "layers": [
      {
        "name": "rescaling_input",
        "class_name": "InputLayer",
        "config": {
          "sparse": false,
          "ragged": false,
          "name": "rescaling_input",
          "dtype": "float32",
          "batch_input_shape": {
            "class_name": "TensorShape",
            "items": [
              null,
              1,
              12
            ]
          }
        },
        "inbound_nodes": []
      },
      {
        "name": "rescaling",
        "class_name": "Rescaling",
        "config": {
          "scale": 0.01,
          "offset": 0.0,
          "name": null,
          "dtype": "float32",
          "batch_input_shape": {
            "class_name": "TensorShape",
            "items": [
              null,
              1,
              12
            ]
          },
          "trainable": true
        },
        "inbound_nodes": [
          [
            "rescaling_input",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense",
        "class_name": "Dense",
        "config": {
          "units": 128,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "rescaling",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_1",
        "class_name": "Dense",
        "config": {
          "units": 128,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_2",
        "class_name": "Dense",
        "config": {
          "units": 128,
          "activation": "relu",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense_1",
            0,
            0
          ]
        ]
      },
      {
        "name": "dense_3",
        "class_name": "Dense",
        "config": {
          "units": 15,
          "activation": "softmax",
          "use_bias": true,
          "kernel_initializer": {
            "class_name": "GlorotUniform",
            "config": {
              "seed": null
            }
          },
          "bias_initializer": {
            "class_name": "Zeros",
            "config": {}
          },
          "kernel_regularizer": null,
          "bias_regularizer": null,
          "kernel_constraint": null,
          "bias_constraint": null,
          "name": null,
          "dtype": "float32",
          "trainable": true
        },
        "inbound_nodes": [
          [
            "dense_2",
            0,
            0
          ]
        ]
      }
    ],
    "input_layers": [
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling_input",
        0,
        0
      ]
    ],
    "output_layers": [
      [
        "rescaling_input",
        0,
        0
      ],
      [
        "rescaling",
        0,
        0
      ],
      [
        "dense",
        0,
        0
      ],
      [
        "dense_1",
        0,
        0
      ],
      [
        "dense_2",
        0,
        0
      ],
      [
        "dense_3",
        0,
        0
      ]
    ]
  },
  "shared_object_id": 1,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      12
    ]
  }
}2
àroot.layer-0"_tf_keras_layer*¶{
  "name": "rescaling",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      12
    ]
  },
  "autocast": false,
  "class_name": "Rescaling",
  "config": {
    "scale": 0.01,
    "offset": 0.0,
    "name": null,
    "dtype": "float32",
    "batch_input_shape": {
      "class_name": "TensorShape",
      "items": [
        null,
        1,
        12
      ]
    },
    "trainable": true
  },
  "shared_object_id": 2,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      12
    ]
  }
}2
óroot.layer_with_weights-0"_tf_keras_layer*¼{
  "name": "dense",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 12
      }
    },
    "shared_object_id": 3
  },
  "class_name": "Dense",
  "config": {
    "units": 128,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 4,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      12
    ]
  }
}2
÷root.layer_with_weights-1"_tf_keras_layer*À{
  "name": "dense_1",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 128
      }
    },
    "shared_object_id": 5
  },
  "class_name": "Dense",
  "config": {
    "units": 128,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 6,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      128
    ]
  }
}2
÷root.layer_with_weights-2"_tf_keras_layer*À{
  "name": "dense_2",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 128
      }
    },
    "shared_object_id": 7
  },
  "class_name": "Dense",
  "config": {
    "units": 128,
    "activation": "relu",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 8,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      128
    ]
  }
}2
úroot.layer_with_weights-3"_tf_keras_layer*Ã{
  "name": "dense_3",
  "trainable": true,
  "expects_training_arg": false,
  "dtype": "float32",
  "batch_input_shape": null,
  "autocast": false,
  "input_spec": {
    "class_name": "InputSpec",
    "config": {
      "DType": null,
      "Shape": null,
      "Ndim": null,
      "MinNdim": 2,
      "MaxNdim": null,
      "Axes": {
        "-1": 128
      }
    },
    "shared_object_id": 9
  },
  "class_name": "Dense",
  "config": {
    "units": 15,
    "activation": "softmax",
    "use_bias": true,
    "kernel_initializer": {
      "class_name": "GlorotUniform",
      "config": {
        "seed": null
      }
    },
    "bias_initializer": {
      "class_name": "Zeros",
      "config": {}
    },
    "kernel_regularizer": null,
    "bias_regularizer": null,
    "kernel_constraint": null,
    "bias_constraint": null,
    "name": null,
    "dtype": "float32",
    "trainable": true
  },
  "shared_object_id": 10,
  "build_input_shape": {
    "class_name": "TensorShape",
    "items": [
      null,
      1,
      128
    ]
  }
}2