# Startup time optimization measurements

| Optimization           | Commit   |  startup time, s  | reference |
|------------------------|----------|:-----------------:|----------:|
| No optimization        | f56deed  |        100        |      1.00 |
| indexOf & substring    | c42977a  |        85         |      0.85 |
| file level parallelism | a0f47ea  |        90         |      0.90 |
| pipelines              | 3ae419b  |        85         |      0.85 |
| all optimizations      | 43c14e1  |        58         |      0.58 |
