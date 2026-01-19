# StoreManager 
Currently working in branch: master 

# Use of AI/LLMs
This project has utilized AI tools to assist in code generation and problem-solving.
Most notably, GitHub Copilot (using Claude Sonnet 4.5) has been used to help generate boilerplate code and suggest improvements throughout the development process.
Most of the initial state of the backend code was made with little to no AI assistance, whatever AI was used amounted to little more than "fancy auto-complete".
The backend requirements and rules given were implemented by a human developer.
The creation of unit tests and integration tests were heavily assisted by AI, followed by manual human review and edits to ensure correctness and adherence to project requirements.
The TestContainers setup making integration testing possible was implemented mainly by a human, with some AI debugging assistance.
The frontend code was almost entirely generated with AI assistance, with a human developer providing structure, requirements, and manual review to ensure the code met the desired functionality and design.
Due to the purpose of this project being to demonstrate my own skills as a developer to OptikIT as part of their interview process, I have chosen to focus on areas where my strenghts lie, namely backend development.
As for unfamiliar areas such as writing tests and frontend development, I have chosen to leverage AI tools to generate code, while leaning heavily on my own knowledge of the domain to guide the AI, review its output, and make necessary adjustments. 

# Docker Compose Setup
To run the application using Docker Compose, ensure you have Docker and Docker Compose installed on your machine. Then, follow these steps:
1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Run the following command to build and start the containers:
    3.1. ```bash
       docker compose up
       ```
4. The api may need to be manually restarted (retry logic have yet to be implemented). Otherwise the swagger ui should be available at: http://localhost:5088/swagger/
5. The db should have seed data already populated to demo.

# Plans
- [x] Setup backend
  - [x] Configure EFCore in the persistence layer so it knows how to map the domain entities to sql tables and vice versa
  - [x] Rule: Ensure a chain's name is unique from other chains
  - [x] Rule: Prevent a chain from being deleted if it still has stores associated with it
  - [x] Rule: Store number must be unique, and will be manually assigned by an OptikIT employee (why manually???)
  - [x] Rule: Stores can be assigned to 0 or 1 chain. If 0 then the store's chainId FK will be null.
  - [x] Rule: A chain can have 1 or more stores
  - [x] Create unit tests for domain and application layer
  - [x] Finish setting up the api
  - [x] Setup logic to facilitate writing and running integration tests
  - [x] Write integration tests for the api
  - [x] Setup docker compose and write instructions for how to run the application in docker
- [ ] Setup frontend
  - [x] Dummy login section that doesn't work unless if I get to the bonus task
  - [ ] Multibutton layout
    - [x] A "Create Chain" page with input text field for the Chain entity and Store entity.
    - [ ] A "Create Store" with text fields for each of the required properties and a pair of radio buttons that turn on/off a searchable dropdown menu for if the store belongs to a chain. If store doesn't belong to a chain then it will appear as "independent"
    - [x] A page that lets the user view a list of chains
      - [x] Subpages for each chain that shows a list of stores belonging to said chain
        - [ ] Create button for adding 1 or more stores to the selected chain, as well as update and delete buttons for each entry
      - [x] Update buttons for each chain
      - [ ] Delete button for each chain
  - [ ] Bonus: Working login functionality
    - [ ] Login with email and password
    - [ ] Rule: Email must be unique
    - [ ] Rule: Employees can be assigned 1 of 2 roles: Administrator or Support
    - [ ] Rule: User "Admin" will always exist with the Administrator role
    - [ ] Rule: "Admin" can never be deleted
    - [ ] Administrator can access all functions in the application
    - [ ] Support can only access function relating to administrating stores and software
    - [ ] UserEntity: Email(string), Name(string), Password(string), Role(string), CreatedOn(DateTime)
