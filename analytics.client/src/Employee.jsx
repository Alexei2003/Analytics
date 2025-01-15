import { useEffect, useState } from 'react';
import './Employee.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole, getEmployeeId } from './funcs/CookiesManager';
import { AddButtonListReportsUser, AddButtonListSurveysWithId, AddButtonListEmployees, AddButtonLogOutWithId, AddButtonAnalyticsUserSurvey } from './funcs/Buttons';

function Employee() {
    const { id } = useParams();
    const [employee, setEmployee] = useState(null);
    const [changeInfo, setChangeInfo] = useState(false);
    const [skills, setSkills] = useState([]);
    const [changeSkills, setChangeSkills] = useState(false);

    const [delSkillsIds, setDelSkillsIds] = useState([]);
    const [newSkills, setNewSkills] = useState([]);

    const [skillName, setSkillName] = useState("");
    const [skillLevel, setSkillLevel] = useState("");

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getEmployee(id);
        getEmployeeSkills(id);
    }, []);

    const handleSetInfo = (event, name) => {
        const { value } = event.target;
        const updatedEmployee = {
            ...employee, 
            [name]: value 
        };
        setEmployee(updatedEmployee);
    };

    function AddLine(name, nameEng) {
        if (changeInfo) {
            return (
                <tr>
                    <td className="text-left">
                        <label>{name}</label>
                    </td>
                    <td className="text-right">
                        <input type="text" value={employee[nameEng]} required onChange={(event) => handleSetInfo(event, nameEng)}></input>
                    </td>
                </tr>
            )
        }

        if (!employee[nameEng]) {
            return null;
        }

        return (
            <tr>
                <td className="text-left">
                    <label>{name}</label>
                </td>
                <td className="text-right">
                    <label> {employee[nameEng]} </label>
                </td>
            </tr>
        )
    }
    function AddImage(url) {
        if (!url) {
            return (
                <img src={"http://localhost/base-images/i.webp"} />
            )
        }

        return (
            <img src={url} />
        )
    }
    function AddButtonChangeInfo() {
        const tokenUserId = getEmployeeId();
        if (tokenUserId === id) {
            if (!changeInfo) {
                return (
                    <button type="button" onClick={() => changeInfoF()} className="panel-btn animate-bounce">Изменить информацию</button>
                );
            }

            if (changeInfo) {
                return (
                    <button type="button" onClick={() => saveInfoF()} className="panel-btn animate-bounce">Сохранить</button>
                );
            }
        }

        return null;

        async function changeInfoF() {
            setChangeInfo(true);
        }

        async function saveInfoF() {
            setChangeInfo(false);
            putInfo(id);
        }
    }
    function AddButtonChangeSkills() {
        const tokenUserId = getEmployeeId();
        if (tokenUserId === id) {
            if (!changeSkills) {
                return (
                    <div className="profile-buttons">
                        <button type="button" onClick={() => changeSkill()} className="panel-btn animate-bounce">Изменить навык</button>
                    </div>
                );
            }

            if (changeSkills) {
                return (
                    <div className="profile-buttons">
                        <button type="button" onClick={() => addSkill()} className="panel-btn animate-bounce">Добавить навык</button>
                        <button type="button" onClick={() => saveSkillsF()} className="panel-btn animate-bounce">Сохранить</button>
                    </div>
                );
            }
        }

        return null;

        async function changeSkill() {
            setChangeSkills(true);
        }

        async function addSkill() {
            var tmp = newSkills;
            tmp = [...newSkills, { id: -1, employeeId: getEmployeeId(), name: skillName, proficiencyLevel: skillLevel }]
            setNewSkills(tmp);
            setSkillName("");
            setSkillLevel("");
        }


        async function saveSkillsF() {
            setChangeSkills(false);
            putSkills(id);
        }
    }
    function AddLineSkill(skill, del ) {
        if (!skill) {
            return null;
        }

        return (
            <tr>
                <td className="text-left">
                    <label>{skill.name}</label>
                </td>
                <td className="text-right">
                    <label> {skill.proficiencyLevel} </label>
                </td>
                {changeSkills && del && (
                    <td>
                        <button type="button" onClick={() => dellSkill(skill.id)} className="panel-btn animate-bounce">Удалить</button>
                    </td>
                )}
            </tr>
        )
    }

    return (
        <>
            {employee && (
                <div className= "container">
                    <div className="profile-header">
                        <div className="profile-picture">
                            {AddImage(employee.PhotoUrl)}
                        </div>
                        <div>
                            <div className="profile-buttons">
                                {AddButtonListSurveysWithId(navigate, id)}
                                {AddButtonListEmployees(navigate)}
                                {AddButtonLogOutWithId(navigate, id)}
                            </div>
                            <div className="profile-buttons">
                                {AddButtonAnalyticsUserSurvey(navigate, id)}
                                {AddButtonChangeInfo()}
                                {AddButtonListReportsUser(navigate, id)}
                            </div>
                        </div>
                    </div>
                    <div className="profile-details">
                        <table>
                            <tbody>
                                {AddLine('Фамилия', "lastName")}
                                {AddLine('Имя',  "firstName")}
                                {AddLine('Отчество', "patronymic")}
                                {AddLine('Пол', "gender")}
                                {AddLine('Дата рождения', "birthDate")}
                                {AddLine('Серия паспорта', "passportSeries")}
                                {AddLine('Номер паспорта', "passportNumber")}
                                {AddLine('Кем выдан паспорт', "passportIssued")}
                                {AddLine('Дата выдачи паспорта', "passportIssuedDate")}
                                {AddLine('Гражданство', "citizenship")}
                                {AddLine('Адрес проживания', "address")}
                                {AddLine('Email', "email")}
                                {AddLine('Телефон', "phoneNumber")}
                                {AddLine('Должность', "position")}
                                {AddLine('Стаж работы в месяцах', "workExperience")}
                                {AddLine('Семейное положение', "maritalStatus")}
                                {AddLine('Ежемесячный доход', "monthlyIncome")}
                            </tbody>
                        </table>
                        <label>Навыки</label>
                        {AddButtonChangeSkills()}
                        {changeSkills && (
                            <div>
                                <label>Название </label>
                                <input type="text" value={skillName} required onChange={(event) => setSkillName(event.target.value)}></input>

                                <label>Уровень</label>
                                <input type="text" value={skillLevel} required onChange={(event) => setSkillLevel(event.target.value)}></input>

                            </div>
                        )}
                        {skills && (
                            <>
                                <table>
                                    <thead>
                                        <tr className="elemListSurveys">
                                            <th>
                                                <label>Название</label>
                                            </th>
                                            <th>
                                                <label>Уровень</label>
                                            </th>
                                            {changeSkills && (
                                                <th>
                                                    <label></label>
                                                </th>
                                            )}
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <>
                                            {skills.map(skill => (
                                                AddLineSkill(skill, true)
                                            ))}
                                            {newSkills.map(skill => (
                                                AddLineSkill(skill, false)
                                            ))}
                                        </>
                                    </tbody>
                                </table>
                            </>
                        )}
                    </div>
                </div>
            )}
        </>
    );

    async function dellSkill(id) {
        var tmp = delSkillsIds;
        tmp = [...delSkillsIds, id]
        setDelSkillsIds(tmp);
        for (var i = 0; i < skills.length; i++) {
            if (skills[i].id == id) {
                skills.splice(i, 1);
                break;
            }
        }
    }

    async function getEmployee(id) {
        try {
            const response = await fetch(`employee/${id}`, { 
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setEmployee(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }

    async function putInfo(id) {
        try {
            const response = await fetch(`employee/${id}`, {
                method: 'PUT',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(employee)
            });

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }

    async function getEmployeeSkills(id) {
        try {
            const response = await fetch(`employee/${id}/skills`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setSkills(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }

    async function putSkills(id) {
        try {
            const response = await fetch(`employee/${id}/skills`, {
                method: 'PUT',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ delSkills: delSkillsIds, newSkills: newSkills })
            });

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
            navigate('/404');
        }
    }
}

export default Employee;