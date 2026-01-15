# StoreManager 
Currently working in branch: master 

# Plans
- [ ] Setup backend
  - [x] Configure EFCore in the persistence layer so it knows how to map the domain entities to sql tables and vice versa
  - [x] Rule: Ensure a chain's name is unique from other chains
  - [ ] Rule: Prevent a chain from being deleted if it still has stores associated with it
  - [x] Rule: Store number must be unique, and will be manually assigned by an OptikIT employee (why manually???)
    - [ ] Experiment with setting the property as auto-increment
  - [ ] Rule: Stores can be assigned to 0 or 1 chain. If 0 then the store's chainId FK will be null.
  - [x] Rule: A chain can have 1 or more stores
  - [ ] Create unit tests for domain and application layer before moving on to the api
  - [ ] Finish setting up the api
  - [ ] Setup logic to facilitate writing and running integration tests
  - [ ] Write integration tests for the api
  - [ ] Setup docker compose and write instructions for how to run the application in docker
- [ ] Setup frontend
  - [ ] Dummy login page that doesn't work unless if I get to the bonus task
  - [ ] Multibutton layout
    - [ ] A "Create Chain" page with input text field for the Chain entity and Store entity. At least one store has to be created along with the chain.
    - [ ] A "Create Store" with text fields for each of the required properties and a pair of radio buttons that turn on/off a searchable dropdown menu for if the store belongs to a chain. If store doesn't belong to a chain then it will appear as "independent"
    - [ ] A page that lets the user view a list of chains
      - [ ] Subpages for each chain that shows a list of stores belonging to said chain
        - [ ] Create button for adding 1 or more stores to the selected chain, as well as update and delete buttons for each entry
      - [ ] Searchbar
      - [ ] Update and delete buttons for each entry
  - [ ] Bonus: Working login functionality
    - [ ] Login with email and password
    - [ ] Rule: Email must be unique
    - [ ] Rule: Employees can be assigned 1 of 2 roles: Administrator or Support
    - [ ] Rule: User "Admin" will always exist with the Administrator role
    - [ ] Rule: "Admin" can never be deleted
    - [ ] Administrator can access all functions in the application
    - [ ] Support can only access function relating to administrating stores and software
    - [ ] UserEntity: Email(string), Name(string), Password(string), Role(string), CreatedOn(DateTime)
