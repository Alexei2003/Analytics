INSERT INTO Roles (role_name) VALUES 
('Admin'), 
('HR'), 
('Employee');

INSERT INTO Employees (
    last_name, 
    first_name, 
    patronymic, 
    birth_date, 
    passport_series, 
    passport_number, 
    passport_issued, 
    passport_issue_date, 
    citizenship, 
    address, 
    email, 
    position, 
    work_experience, 
    marital_status, 
    monthly_income,
    gender
) VALUES
('Петров', 'Алексей', 'Александрович', '1985-05-15', 'AB', '123456', 'Управление внутренних дел', '2005-06-01', 'Беларусь', 'Минск, ул. Ленина, 1', 'ivanov@gmail.com', 'Разработчик', 10, 'Женатый', 2500.00, 'Мужской'),
('Петров', 'Петр', 'Петрович', '1990-03-20', 'CD', '654321', 'Управление внутренних дел', '2010-04-15', 'Беларусь', 'Минск, ул. Октябрьская, 2', 'petrov@gmail.com', 'Аналитик', 5, 'Холост', 2000.00, 'Мужской'),
('Сидорова', 'Анна', 'Викторовна', '1988-11-30', 'EF', '789012', 'Управление внутренних дел', '2008-12-01', 'Беларусь', 'Гомель, ул. Советская, 3', 'sidorova@gmail.com', 'Менеджер проектов', 6, 'Замужняя', 3000.00, 'Женский'),
('Коваленко', 'Алексей', 'Сергеевич', '1982-01-15', 'GH', '345678', 'Управление внутренних дел', '2002-02-10', 'Беларусь', 'Брест, ул. Мира, 4', 'kovalenko@gmail.com', 'Тестировщик', 12, 'Женатый', 2200.00, 'Мужской'),
('Смирнова', 'Елена', '', '1995-07-25', 'IJ', '901234', 'Управление внутренних дел', '2015-08-05','Беларусь','Витебск, ул. Победы, 5','smirnova@gmail.com','Дизайнер интерфейсов',3,'Незамужняя',1800.00,'Женский'),
('Федоров','Дмитрий','Александрович','1987-04-10','KL','567890','Управление внутренних дел','2007-05-20','Беларусь','Могилев, ул. Ленина, 6','fedorov@gmail.com','Системный администратор',8,'Женатый',2600.00,'Мужской'),
('Григорьева','Мария','Викторовна','1992-09-14','MN','234567','Управление внутренних дел','2012-10-25','Беларусь','Гродно, ул. Горького, 7','grigorieva@gmail.com','Бизнес-анализатор',4,'Незамужняя',2300.00,'Женский'),
('Орлов','Андрей','Михайлович','1984-12-05','OP','890123','Управление внутренних дел','2004-01-15','Беларусь','Барановичи, ул. Солнечная, 8','orlov@gmail.com','Менеджер проектов',11,'Холост',2700.00,'Мужской');

INSERT INTO Skills (employee_id, skill_name, proficiency_level) VALUES
(1, 'JavaScript Programming', 9),
(1, 'React Development', 8),
(2, 'Data Analysis', 7),
(3, 'Project Management', 6),
(4, 'Quality Assurance Testing', 8),
(5, 'UI/UX Design Principles', 7),
(6, 'System Administration Basics', 9),
(7, 'Business Analysis Techniques', 6),
(8, 'Software Development Lifecycle (SDLC)', 7);

INSERT INTO Users (user_id, employee_id, role_id, username, password_hash, salt, created_at, last_login) VALUES
(1, 1, 1, 'petrov_admin', 'EcLKD0VJpzPe6EoPb9YmPFOgJlxhastYe++Yufqc8mY=', decode('8D89CEF21201827F06405383AF7B5A25', 'hex'), '2023-01-15 10:30:00', '2023-11-01 08:45:00'),
(2, 2, 2, 'petrov_hr', 'eAqbK6G195eW7FHjvP0uWbdXDTMNSflCYUtbRxbsNAI=', decode('84cc6b934e78c608c051bb6415963008', 'hex'), '2023-02-20 14:15:00', '2023-11-05 09:30:00'),
(3, 3, 3, 'sidorova_employee', 'CNWfU13b6Yk5LZhVvUy0aNRdxMWaxuhUJDr9PkQiRys=', decode('0e0aa1c28684c60a1e599586ba58e9c3', 'hex'), '2023-03-10 09:00:00', NULL),
(4, 4, 3, 'kovalenko_employee', 'AbftSgW9jjuw2u1Vzwhb7mA+TiVEFvcDozc3XVqqBWY=', decode('d78b38f8b7e521636665ec3169c4ebb0', 'hex'), '2023-04-25 11:45:00', '2023-11-10 12:00:00'),
(5, 5, 3, 'smirnova_employee', 'nCQD02OYrOm0/BYCr2LJCvvACOZA2+C5JDDte1IwuQU=', decode('121ec9571a79738a64c7070006607501', 'hex'), '2023-05-30 13:30:00', NULL),
(6, 6, 3, 'fedorov_employee', 'ltlQrOaYptthwXFR0nNZkGxV7NLXEcKllRvNYldtzWQ=', decode('0377756ec1ddf6dc0248e7a733dca768', 'hex'), '2023-06-15 15:00:00', '2023-11-15 14:30:00'),
(7, 7, 3, 'grigorieva_employee', '1Fltibu4phX0buQkPynK6vgL0/0E0g5yxEPBWD0jZWU=', decode('594df4c5da199739370e6fb7316edabd', 'hex'), '2023-07-20 16:20:00', NULL),
(8, 8, 3, 'orlov_employee', 'oMfwOZGTWHVmIU0NRCOPHiOPKMDxLuhp7bFDue8BXk8=', decode('c3adf59e1e7d99aceff0b8bf96144e7e','hex'), '2023-08-05 17:10:00', '2023-11-20 16:40:00');

INSERT INTO Event_Categories (category_name) VALUES 
('Training'), 
('Workshop'), 
('Conference'), 
('Webinar');

INSERT INTO Reports (
    title,
    details,
    date,
    category_id,
    location
) VALUES
('Основы C#', 'Интенсивный курс по основам программирования на C#.', '2024-01-10', 1, 'Минск'),
('Современные подходы к тестированию', 'Семинар о лучших практиках тестирования программного обеспечения.', '2024-02-15', 2, 'Гомель'),
('Проектирование баз данных', 'Бонусная сессия по проектированию эффективных баз данных.', '2024-03-20', 3, 'Брест'),
('Преимущества Agile', 'Обсуждение преимуществ Agile методологий в разработке.', '2024-04-25', 2, 'Могилев'),
('Индивидуальная консультация по карьерному росту', 'Персональная консультация для обсуждения карьерных возможностей.', '2024-05-30', 4, 'Витебск'),
('React для начинающих', 'Курс для изучения основ React и создания простых приложений.', '2024-06-10', 1, 'Минск'),
('DevOps в разработке ПО', 'Семинар о внедрении DevOps практик в процесс разработки.', '2024-07-15', 3, 'Гродно'),
('Новые тренды в IT', 'Бонусный вебинар о последних трендах в области информационных технологий.', '2024-08-20', 4, 'Могилев');

INSERT INTO Reports_And_Users (
	employee_id,
	report_id
) VALUES
(1,1),
(1,2),
(1,3),
(2,2),
(3,3),
(4,4),
(5,5),
(6,6),
(7,7),
(8,8);