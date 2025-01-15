/*DROP DATABASE IF EXISTS analytics;*/

CREATE TABLE Roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Employees (
    employee_id SERIAL PRIMARY KEY,
    last_name VARCHAR(50) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    patronymic VARCHAR(50),
    birth_date DATE NOT NULL,
    passport_series VARCHAR(10) NOT NULL,
    passport_number VARCHAR(10) NOT NULL,
    passport_issued VARCHAR(100) NOT NULL,
    passport_issue_date DATE NOT NULL,
    citizenship VARCHAR(50) NOT NULL,
    address VARCHAR(255),
    email VARCHAR(100) UNIQUE NOT NULL,
    position VARCHAR(50) NOT NULL,
    work_experience INT CHECK (work_experience >= 0),
    marital_status VARCHAR(20) CHECK (marital_status IN ('Женатый', 'Замужняя', 'Холост', 'Незамужняя')),
    monthly_income DECIMAL(10, 2) CHECK (monthly_income >= 0),
    gender VARCHAR(20) CHECK (gender IN ('Мужской', 'Женский'))
);

CREATE TABLE Skills (
    skill_id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    skill_name VARCHAR(100) NOT NULL,
    proficiency_level INT CHECK (proficiency_level BETWEEN 1 AND 10),
    FOREIGN KEY (employee_id) REFERENCES Employees(employee_id)
);

CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    role_id INT NOT NULL,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    salt BYTEA NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP,
    
    FOREIGN KEY (employee_id) REFERENCES Employees(employee_id),
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

CREATE TABLE Event_Categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Reports (
    report_id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
	details TEXT,
	date DATE NOT NULL,
	category_id INT NOT NULL,
	location VARCHAR(100),
	
	FOREIGN KEY (category_id) REFERENCES Event_Categories(category_id)
);

CREATE TABLE Reports_And_Users(
	employee_id INT NOT NULL,
	report_id INT NOT NULL,
	
	FOREIGN KEY (employee_id) REFERENCES Employees(employee_id),
	FOREIGN KEY (report_id) REFERENCES Reports(report_id)

);

CREATE TABLE Surveys (
    survey_id SERIAL PRIMARY KEY,
    title VARCHAR(100) NOT NULL,
    description TEXT,
    image_url VARCHAR(255),
    creation_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP
);

CREATE TABLE Survey_Questions (
	question_id SERIAL PRIMARY KEY,
	survey_id INT NOT NULL,
	question_text TEXT NOT NULL,
	FOREIGN KEY (survey_id) REFERENCES Surveys(survey_id)
);

CREATE TABLE Survey_Responses (
	response_id SERIAL PRIMARY KEY,
	question_id INT NOT NULL,
	answer VARCHAR(255),
	score INT CHECK (score >= 1 AND score <= 5),
	FOREIGN KEY (question_id) REFERENCES Survey_Questions(question_id)
);

CREATE TABLE Survey_Responses_And_Users (
	response_id INT NOT NULL,
	employee_id INT NOT NULL,
	FOREIGN KEY (response_id) REFERENCES Survey_Responses(response_id),
	FOREIGN KEY (employee_id) REFERENCES Employees(employee_id)
);
