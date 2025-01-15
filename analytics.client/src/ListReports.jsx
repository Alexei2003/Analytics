import { useEffect, useState } from 'react';
//import './ListReports.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole, getEmployeeId } from './funcs/CookiesManager';
import { AddButtonAdminPanel, AddButtonUserProfile, AddButtonHRPanel, AddButtonAddReport } from './funcs/Buttons';

function ListReports() {

    const { employeeId } = useParams();

    const [listReports, setListReports] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getListReports();
    }, []);

    function AddLine(report) {
        if (!report) {
            return null;
        }

        return (
            <tr onClick={() => navigateReport(report.id)} key={report.id} className="elemListSurveys">
                <td>
                    <label>{report.title || 'Не указано'}</label>
                </td>
                <td>
                    <label>{report.date || 'Не указано'}</label>
                </td>
                <td>
                    <label>{report.location || 'Не указано'}</label>
                </td>
            </tr>
        );
    }
 
    return (
        <div className="container  animate-fade-in">
            <div className="profile-buttons">
                {AddButtonHRPanel(navigate)}
                {AddButtonAdminPanel(navigate)}
                {AddButtonAddReport(navigate, employeeId)}
                {AddButtonUserProfile(navigate)}
            </div>

            <table className="tableListSurveys">
                <thead>
                    <tr className="elemListSurveys">
                        <th>
                            <label>Название</label>
                        </th>
                        <th>
                            <label>Дата провидения</label>
                        </th>
                        <th>
                            <label>Место провидения</label>
                        </th>
                    </tr>
                </thead>

                <tbody>
                    {listReports ? (
                        <>
                            {listReports.map(report => (
                                AddLine(report)
                            ))}
                        </>
                    ) : null}
                </tbody>
            </table>
        </div>
    );

    async function navigateReport(reportId) {
        navigate(`/report/${reportId}/${employeeId}`);
    }

    async function getListReports() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`listreports?employeeId=${employeeId}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setListReports(data);
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

export default ListReports;