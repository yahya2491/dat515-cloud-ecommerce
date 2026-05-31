# Project Evaluation Checklist

The group earn points by completing items from the categories below.
You are not expected to complete all items.
Focus on areas that align with your project goals and interests.

The core deliverables are required.
This means that you must get at least 2 points for each item in this category.

| **Category**                     | **Item**                                | **Max Points** | **Points** | **Comment** |
| -------------------------------- | --------------------------------------- | -------------- | ---------- | ----------- |
| **Core Deliverables (Required)** |                                         |                |            |             |
| Codebase & Organization          | Well-organized project structure        | 5              | 5          | Backend & frontend have clear modular structure and consistent naming. |
|                                  | Clean, readable code                    | 5              | 5          | Code formatted, minimal clutter, readable functions. |
|                                  | Use planning tool (e.g., GitHub issues) | 5              | 5          | GitHub issues used to track tasks and progress. |
|                                  | Proper version control usage            | 5              | 2          | Some commits lack clear messages / branching discipline. |
|                                  | Complete source code                    | 5              | 5          | All source code present and organized. |
| Documentation                    | Comprehensive reproducibility report    | 10             | 7         | Report includes run steps; could be more detailed for local reproducibility. |
|                                  | Updated design document                 | 5              | 5          | Recently updated reflecting current architecture. |
|                                  | Clear build/deployment instructions     | 5              | 5          | Instructions are concise and easy to follow. |
|                                  | Troubleshooting guide                   | 5              | 5          | Common issues encountered and solutions listed. |
|                                  | Completed self-assessment table         | 5              | 5          | Table filled, mirrors project status. |
|                                  | Hour sheets for all members             | 5              | 5          | All hour sheets complete and submitted. |
| Presentation Video               | Project demonstration                   | 5              | 5          | Shows major features and user flows. |
|                                  | Code walk-through                       | 5              |   4         |     Doesnt show any code examples in the backend        |
|                                  | Deployment showcase                     | 5              |   5         |      Deployment process is clearly shown.       |
| **Technical Implementation**     |                                         |                |            |             |
| Application Functionality        | Basic functionality works               | 10             | 10         | Register, login, browse products, cart ops, checkout, clear cart. |
|                                  | Advanced features implemented           | 10             | 5          | Product recommendations via similarity matching. |
|                                  | Error handling & robustness             | 10             | 10         | Graceful handling of failed login, 400s, empty cart, payment and validation errors. |
|                                  | User-friendly interface                 | 5              | 5          | Clean UI, intuitive routing, simple forms, confirmation messages. |
| Backend & Architecture           | Stateless web server                    | 5              |     5       |     All requests are self contained and do not rely on server state.        |
|                                  | Stateful application                    | 10             |      5      |      Products, users, etc. are persistence between requests but the database itself is not.      |
|                                  | Database integration                    | 10             |      10      |  Postgresql is integrated           |
|                                  | API design                              | 5              |    5        |   The design of the API is intuitive and follows REST principles.           |
|                                  | Microservices architecture              | 10             |     8       |  Split into frontend, backend, database, and monitoring services.           |
| Cloud Integration                | Basic cloud deployment                  | 10             |     10       | deployed the full stack (DB, API, Frontend, Grafana, Prometheus) on OpenStack using Talos + Kubernetes            |
|                                  | Cloud APIs usage                        | 10             |      7      |   used OpenStack features like Floating IPs, VM provisioning, and security groups.          |
|                                  | Serverless components                   | 10             |       0     |             |
|                                  | Advanced cloud services                 | 5              |       3     |  implemented full Kubernetes orchestration, multi-replica deployments, internal DNS, floating IP routing, and monitoring stack           |
| **DevOps & Deployment**          |                                         |                |            |             |
| Containerization                 | Basic Dockerfile                        | 5              |    5        |    Docker file implemented          |
|                                  | Optimized Dockerfile                    | 5              |     3      |  We used multi-stage Docker builds for both the API and the frontend, which significantly reduces image size           |
|                                  | Docker Compose                          | 5              |      5      |  Completed a full docker-compose setup to run for local deployment and tests that runs the entire project services with one command           |
|                                  | Persistent storage                      | 5              |   0         |   No persistent storage implemented.           |
| Deployment & Scaling             | Manual deployment                       | 5              |     5       |    We use manual deployment to get the containers up and running on OpenStack.         |
|                                  | Automated deployment                    | 5              |      5      |      We use CI/CD pipelines for automated deployment (on DockerHub) via GitHub Actions.       |
|                                  | Multiple replicas                       | 5              |      5      |  Our Kubernetes deployment uses multiple replicas for both the API and the frontend.           |
|                                  | Kubernetes deployment                   | 10             |    10        | Kubernetes deployment implemented in the OpenStack VM             |
| **Quality Assurance**            |                                         |                |            |             |
| Testing                          | Unit tests                              | 5              |     5      |  Unit tests were created in the backend            |
|                                  | Integration tests                       | 5              |    0        |         No integration tests    |
|                                  | End-to-end tests                        | 5              |     0       |    No end to end tests          |
|                                  | Performance testing                     | 5              |      5      |      Performance tests created. |
| Monitoring & Operations          | Health checks                           | 5              |      5      |     Health checks for stripe and database integration        |
|                                  | Logging                                 | 5              |     5       |        In depth logging, we also have a /metrics endpoint that shows logs      |
|                                  | Metrics/Monitoring                      | 2              |      2      |      Graphana implemented        |
|                                  | Custom metrics for your project         | 3              |     3       |      We have custom metrics for the Admin, including user activity.       |
| Security                         | HTTPS/TLS                               | 5              |   0         |      No https implemented       |
|                                  | Authentication                          | 5              | 5          | Login system implemented on frontend. |
|                                  | Authorization                           | 5              | 5          | Role-based redirects provide admin access control. |
| **Innovation & Excellence**      |                                         |                |            |             |
| Advanced Features and            | AI/ML Integration                       | 10             |     5       |     AI/ML integration used to catagorize products when creating them        |
| Technical Excellence             | Real-time features                      | 10             |     0       |   No real time features were implemented.          |
|                                  | Creative problem solving                | 10             |      10      |   Products are recommended to users on checkout using products in the same catagory + products are catagorized using AI           |
|                                  | Performance optimization                | 5              |      0      |  No performance optimizations were implemented.            |
|                                  | Exceptional user experience             | 5              | 3          | Smooth navigation, clear error/success messaging. However the frontend still has areas of improvment |
| **Total**                        |                                         | **255**        | 242 |             |

## Grading Scale

- **Minimum Required: 100 points**
- **Maximum: 200+ points**

| Grade | Points   |
| ----- | -------- |
| A     | 180-200+ |
| B     | 160-179  |
| C     | 140-159  |
| D     | 120-139  |
| E     | 100-119  |
| F     | 0-99     |
