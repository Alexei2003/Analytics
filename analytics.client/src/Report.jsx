import { useEffect, useState } from 'react';
//import './Report.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole, getEmployeeId } from './funcs/CookiesManager';
import { AddButtonHRPanel, AddButtonAdminPanel, AddButtonUserProfile, AddButtonListReportsUser } from './funcs/Buttons';

function Report() {

    const { reportId, employeeId } = useParams();

    const [report, setReport] = useState(null);
    const [employeesList, setEmployeesList] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        if (reportId !== "0") {
            getReport();
        }
        else {
            setReport({ id: -1, title: "", details: "", data: new Date, location: "", categoryName: "Training", employees: [] });
            getEmployeesList();
        }
    }, []);

    const handleSetInfo = (event, name) => {
        const { value } = event.target;
        const updatedReport= {
            ...report,
            [name]: value
        };
        setReport(updatedReport);
    };

    function AddLine(name, nameEng, date = false) {

        if (reportId === "0") {
            return (
                <tr>
                    <td className="text-left">
                        <label>{name}</label>
                    </td>
                    <td className="text-right">
                        {date && (
                            <input type="date" value={report[nameEng]} required onChange={(event) => handleSetInfo(event, nameEng)}></input>
                        ) }
                        {!date && (
                            <input type="text" value={report[nameEng]} required onChange={(event) => handleSetInfo(event, nameEng)}></input>
                        )}
                    </td>
                </tr>
            )
        }

        if (!report[nameEng]) {
            return null;
        }

        return (
            <tr>
                <td className="text-left">
                    <label>{name}</label>
                </td>
                <td className="text-right">
                    <label> {report[nameEng]} </label>
                </td>
            </tr>
        )
    }

    function AddLineEmployee(employee) {
        if (!employee) {
            return null;
        }

        return (
            <tr onClick={() => reportId !== "0" ? navigateToEmployee(employee.id) : addEmployee(employee.id)} key={employee.id} className="elemListEmployees">
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
            </tr>
        );
    }

    return (
        <div className="container  animate-fade-in">
            <div className="profile-buttons">
                {AddButtonHRPanel(navigate)}
                {AddButtonAdminPanel(navigate)}
                {AddButtonListReportsUser(navigate, employeeId)}
                {AddButtonUserProfile(navigate)}
            </div>

            {report && reportId !== "0" && (
                <div className="profile-details">
                    <h3>{report.title}</h3>

                    <label>{report.details}</label>

                    <table>
                        <tbody>
                            {AddLine('Дата мероприятия', "date")}
                            {AddLine('Место провидения', "location")}
                            {AddLine('Тип мероприятия', "categoryName")}
                        </tbody>
                    </table>

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
                            </tr>
                        </thead>

                        <tbody>
                            {report.employees && (
                                <>
                                    {report.employees.map(employee => (
                                        AddLineEmployee(employee)
                                    ))}
                                </>
                            )}
                        </tbody>
                    </table>
                </div>
            )}

            {report && reportId === "0" && (
                <>
                    <div className="profile-buttons">
                        <button type="button" onClick={() => sendReport()} className="panel-btn animate-bounce">Сохранить</button>
                    </div>
                    <table>
                        <tbody>
                            {AddLine('Название', "title")}
                            {AddLine('Описание', "details")}
                            {AddLine('Дата мероприятия', "date", true)}
                            {AddLine('Место провидения', "location")}
                            {AddLine('Тип мероприятия', "categoryName")}
                        </tbody>
                    </table>

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
                            </tr>
                        </thead>

                        <tbody>
                            {employeesList && (
                                <>
                                    {employeesList.map(employee => (
                                        AddLineEmployee(employee)
                                    ))}
                                </>
                            )}
                        </tbody>
                    </table>
                </>
            )}
        </div>
    );

    async function getReport() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`report/${reportId}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setReport(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }

    async function sendReport() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`report`, {
                method: 'POST',
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(report)
            });

            if (response.ok) {
                navigate(`/listreports/${employeeId}`);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }

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

    async function addEmployee(id) {
        const updatedEmployeesList = employeesList.filter(employee => employee.id !== id);
        setEmployeesList(updatedEmployeesList);

        const tmp = report;
        tmp.employees = [...report.employees, { id: id, lastName: "", firstName: "", patronymic: "", position: "", role: "" }]
        setReport(tmp);
    }

}

export default Report;