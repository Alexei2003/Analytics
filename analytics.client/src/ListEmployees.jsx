import { useEffect, useState } from 'react';
import './ListEmployees.css';
import { useNavigate } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import { AddButtonAdminPanel, AddButtonHRPanel, AddButtonUserProfile } from './funcs/Buttons';

function ListEmployees() {
    const [employeesList, setEmployeesList] = useState(null);
    const [userRole, setUserRole] = useState(null);
    const [changeRole, setChangeRole] = useState(false);
    const [listSurveys, setListSurveys] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        const role = getUserRole();
        if (!role) {
            navigate('/');
        }
        setUserRole(role);
        getEmployeesList();
        getListSurveys();
    }, []);

    useEffect(() => {
        if (listSurveys !== null) {
            const timer = setTimeout(() => {
                setListSurveys(null);
            }, 10000); // 10 секунд
        }
    }, [listSurveys]);

    const [newRoles, setNewRoles] = useState([]);
    const handleChangeRole = (event) => {
        const tmp = JSON.parse(event.target.value);
        for (let i = 0; i < employeesList.length; i++) {
            if (employeesList[i].id === tmp.employeeId) {
                employeesList[i].role = tmp.role;
                break;
            }
        }
        const tmpArr = [...newRoles, tmp];
        setNewRoles(tmpArr);
    };

    function AddLine(employee) {
        if (!employee) {
            return null;
        }

        return (
            <tr onClick={() => navigateToEmployee(employee.id)} key={employee.id} className="elemListEmployees">
                <td>
                    <label>{employee.lastName || 'Не указано'}</label>
                </td>
                <td>
                    <label>{employee.firstName || 'Не указано'}</label>
                </td>
                <td>
                    <label>{employee.patronymic || 'Не указано'}</label>
                </td>
                <td>
                    <label>{employee.position || 'Не указано'}</label>
                </td>
                {userRole === "Admin" && changeRole && (
                    <td>
                        <select onChange={handleChangeRole} value={JSON.stringify({ employeeId: employee.id, role: employee.role})}>
                            <option value={JSON.stringify({ employeeId: employee.id, role: "Admin" })}>
                                {"Админ"}
                            </option>
                            <option value={JSON.stringify({ employeeId: employee.id, role: "HR" })}>
                                {"HR"}
                            </option>
                            <option value={JSON.stringify({ employeeId: employee.id, role: "Employee" })}>
                                {"Работник"}
                            </option>
                        </select>
                    </td>
                )}
            </tr>
        );
    }

    return (
        <>
            {listSurveys !== null && listSurveys.length !== 0 && (
                <div className="chart-container notification-container profile-details">
                    <h3 className="text-left"> Уведомление </h3>
                    <label>Вы не прошли опрос. Если у вас возникли трудности, обратитесь за помощью к нашим специалистам.</label>
                </div>
            ) }
            
            <div className="container  animate-fade-in">
                <div className="profile-buttons">
                    {AddButtonAdminPanel(navigate)}
                    {AddButtonHRPanel(navigate)}
                    {userRole === "Admin" && !changeRole && (
                        <th>
                            <button type="button" onClick={() => changeRoleF()} className="analytics-btn animate-bounce">Управлять сотрудниками</button>
                        </th>
                    )}
                    {userRole === "Admin" && changeRole && (
                        <th>
                            <button type="button" onClick={() => saveRole()} className="analytics-btn animate-bounce">Сохранить</button>
                        </th>
                    )}
                    {AddButtonUserProfile(navigate)}
                </div>
                <table className="tableListEmployees">
                    <thead>
                        <tr className="elemListEmployees">
                            <th>
                                <label>Фамилия</label>
                            </th>
                            <th>
                                <label>Имя</label>
                            </th>
                            <th>
                                <label>Отчество</label>
                            </th>
                            <th>
                                <label>Позиция</label>
                            </th>
                            {userRole === "Admin" && changeRole && (
                                <th>
                                    <label>Роль</label>
                                </th>
                            )}
                        </tr>
                    </thead>

                    <tbody>
                        {employeesList && (
                            <>
                                {employeesList.map(employee => (
                                    AddLine(employee)
                                ))}
                            </>
                        )}
                    </tbody>
                </table>
            </div>
        </>
    );

    async function getEmployeesList() {
        try {
            const response = await fetch(`listemployees`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setEmployeesList(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }

    async function navigateToEmployee(id) {
        if (!changeRole) {
            navigate(`/employee/${id}`); 
        }
    }

    async function changeRoleF() {
        setChangeRole(true);
    }

    async function saveRole() {
        try {
            const response = await fetch(`listemployees`, {
                method: 'PUT',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(newRoles),
            });

            if (response.ok) {
                setChangeRole(false);
                setNewRoles([]);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }

    async function getListSurveys() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`listsurveys?type=${-1}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setListSurveys(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }
}

export default ListEmployees;